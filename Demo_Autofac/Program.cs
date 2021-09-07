using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Demo_Autofac.Aop;
using Demo_Autofac.Repositories;
using Demo_Autofac.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//使用Autofac作为默认IOC容器
//如果使用Setup.cs作为启动配置，则需要在Setup.cs中创建  public void ConfigureContainer(ContainerBuilder builder){} 方法
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    //单例计数服务 观察 MathService、UseMathService、BaseRepository 被创建的次数
    //以下MathService的注入 最后一条实现，测试时可以从上倒下注释
    builder.RegisterType<ResolveCountService>().As<IResolveCountService>().SingleInstance();

    //单例模式
    builder.RegisterType<MathService>().As<IMathService>().SingleInstance();

    //每次依赖都会创建新的实例
    builder.RegisterType<MathService>().As<IMathService>().InstancePerDependency();

    //整个请求生命周期中创建一次
    builder.RegisterType<MathService>().As<IMathService>().InstancePerLifetimeScope();

    //根据程序集搜索、筛选类型进行依赖注入
    //IBaseRepository 在 MathService中被注入 观察解析时需要参考IMathService的解析
    builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
           .PublicOnly()
           .Where(t => t.Name.EndsWith("Repository"))
           .Except<UnUseRepository>()
           .AsImplementedInterfaces()
           .InstancePerDependency();

    builder.RegisterType<UseMathService>().As<IUseMathService>().InstancePerLifetimeScope();


    //其他好用的注入方式

    //泛型动态注入 并实现Aop
    builder.RegisterGeneric(typeof(TemplateService<,>))
        .As(typeof(ITemplateService<,>))
        .EnableInterfaceInterceptors();

    //注入个人实现的aop
    builder.RegisterType(typeof(CustomAutofacAop));

    //使用aop实现efcore版本的unit of work
    //注入拦截器类型
    builder.RegisterType(typeof(UnitOfWorkInterceptor));

    //注入uow
    builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

    //注入使用uow的service
    builder.RegisterType<EFCoreAopStudentService>()
        .As<IEFCoreAopStudentService>()
        .InstancePerLifetimeScope()
        .EnableInterfaceInterceptors()
        .InterceptedBy(typeof(UnitOfWorkInterceptor));


    #region 注册所有控制器的关系及控制器实例化所需要的组件

    var controllersTypesInAssembly = Assembly.GetExecutingAssembly().GetExportedTypes()
        .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToArray();

    builder.RegisterTypes(controllersTypesInAssembly)
        .PropertiesAutowired();

    #endregion
}));

builder.Services.AddDbContext<AppDbContext>(builder =>
{
    builder.UseInMemoryDatabase("test");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Demo_Autofac", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo_Autofac v1"));
}

#region api

/// <summary>
/// 测试依赖注入的几种生命周期
/// </summary>
app.MapGet("math/add", (double x1, double x2, [FromServices] IMathService mathService, [FromServices] IUseMathService useMathService) =>
{
useMathService.Add(x1, x2);

return Results.Ok(mathService.Add(x1, x2));
});

/// <summary>
/// 测试注入方式
/// </summary>
app.MapGet("test/others",async ([FromServices] ITemplateService<int, string> treeBaseService) =>
{
    treeBaseService.GetNode(1);

    return Results.Ok(await treeBaseService.GetKeyAsync(1));
});

#region Aop for ef uow 

app.MapGet("aop/students", async ([FromServices]IEFCoreAopStudentService studentService) =>
{
    return Results.Ok(await studentService.GetAsync());
});

app.MapGet("aop/students/{id}", async ([FromRoute] int id, [FromServices] IEFCoreAopStudentService studentService) =>
{
    return Results.Ok(await studentService.GetAsync(id));
});

app.MapPost("aop/students", async ([FromBody] Student student,[FromServices] IEFCoreAopStudentService studentService) =>
{
    await studentService.CreateAsync(student);
    return Results.Ok(await studentService.GetAsync());
});

app.MapPut("aop/students/{id}", async ([FromRoute] int id, [FromBody] string name, [FromServices] IEFCoreAopStudentService studentService) =>
{
    await studentService.UpdateAsync(id, name);
    return Results.Ok(await studentService.GetAsync());
});

app.MapDelete("aop/students/{id}", async ([FromRoute] int id,[FromServices] IEFCoreAopStudentService studentService) =>
{
    await studentService.DeleteAsync(id);
    return Results.Ok(await studentService.GetAsync());
});

#endregion

#endregion

app.Run();
