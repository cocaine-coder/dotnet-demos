using Autofac;
using Autofac.Extensions.DependencyInjection;
using Demo_Autofac.Repositories;
using Demo_Autofac.Services;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//使用Autofac作为默认IOC容器
//如果使用Setup.cs作为启动配置，则需要在Setup.cs中创建  public void ConfigureContainer(ContainerBuilder builder){} 方法
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    //单例计数服务 观察 MathService、UseMathService、BaseRepository 被创建的次数
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

    //泛型动态注入 
    builder.RegisterGeneric(typeof(TemplateService<,>))
        .As(typeof(ITemplateService<,>));
}));

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

// 测试注入方式
app.MapGet("test/others", ([FromServices] ITemplateService<int, string> treeBaseService) =>
{
    treeBaseService.GetNode(1);

    return Results.Ok();
}); 
#endregion

app.Run();
