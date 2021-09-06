using Demo_Jwt;
using Demo_Jwt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddJwtAuthentication(builder.Configuration)
    .AddCustomAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IJwtService, JwtService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Demo", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            System.Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo v1"));
}

app.UseAuthentication();
app.UseAuthorization();

#region api

/// <summary>
/// 模拟颁发token
/// 生产中，拒绝提供role和isForever关键字
/// SecurityJwtConfig.Forever指定该token永久有效<see cref="AuthExtension"/>
/// 确认身份后颁发token,token 可携带用户id，用户名等信息(可公开的)
/// </summary>
app.MapGet("token", ([FromQuery] Roles role, [FromQuery] bool isForever, IJwtService jwtService) =>
{
    var claims = new List<Claim>()
    {
        new Claim(JwtRegisteredClaimNames.Sub, "foo"),
        new Claim(ClaimTypes.Role, role.ToString())
    };

    if (isForever)
        claims.Add(new Claim(SecurityJwtConfig.Forever, ""));

    return Results.Ok(jwtService.GenerateToken(claims));
});


#region 测试认证方式

/// <summary>
/// token放置在url中作为参数传递，方便一些链接类的资源使用
/// 这里将access_token显示地放置在action函数参数中，方便swagger测试
/// </summary>
app.MapGet("token_in_url", [Authorize]([FromQuery] string access_token, IJwtService jwtService) =>
{
    return Results.Ok(jwtService.ResolveToken(access_token));
});

/// <summary>
/// token放置在request的header中
/// </summary>
app.MapGet("token_in_header", [Authorize](IHttpContextAccessor accessor, IJwtService jwtService) =>
{
    StringValues token = new();
    var ret = accessor.HttpContext?.Request.Headers.TryGetValue("Authorization", out token);
    if (ret == null || !ret.Value)
        return Results.BadRequest("request header can not find Authorization option!");

    return Results.Ok(jwtService.ResolveToken(token.First().Split(' ').Last()));
});

#endregion

#region 测试授权

app.MapGet("authorize/root", [Authorize(policy: nameof(Roles.ROOT))](string access_token) =>
{
    return Results.Ok(Roles.ROOT);
});

app.MapGet("authorize/admin", [Authorize(policy: nameof(Roles.ADMIN))](string access_token) =>
{
    return Results.Ok(Roles.ADMIN);
});

app.MapGet("authorize/normal", [Authorize(policy: nameof(Roles.NORMAL))](string access_token) =>
{
    return Results.Ok(Roles.NORMAL);
});

#endregion 

#endregion

app.Run();
