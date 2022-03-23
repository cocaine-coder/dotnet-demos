
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Demo_Jwt;
public static class AuthExtension
{
    /// <summary>
    /// Jwt认证实现
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = new SecurityJwtConfig();
        configuration.GetSection("Security:Jwt").Bind(jwtConfig);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(jwtConfig.KeyBytes),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents()
            {
                //认证失败handler
                OnAuthenticationFailed = context =>
                {
                    //如果token过期在返回头上添加过期标识
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        context.Response.Headers.Add("Token-Expired", "true");

                    return Task.CompletedTask;
                },

                //请求在未认证之前的handler
                OnMessageReceived = context =>
                {
                    context.Request.Headers.TryGetValue("Authorization", out var token);
                    if (!string.IsNullOrEmpty(token))
                        token = token.First().Split(' ').Last();

                    //如果query中不存在access_token则在request header中查找，为下面修改过期策略做准备
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        //如果query参数中存在access_token则将该值作为认证token传递
                        if (context.Request.Query.TryGetValue("access_token", out token))
                            context.Token = token;
                    }

                    //如果请求中确实存在token则验证token中是否存在SecurityJwtConfig.Forever的claim 存在=>不判断过期  不存在=>判断过期
                    if (!string.IsNullOrWhiteSpace(token) &&
                        new JwtSecurityTokenHandler().ReadJwtToken(token)?.Claims?.FirstOrDefault(x => x.Type == SecurityJwtConfig.Forever) != null)
                        options.TokenValidationParameters.ValidateLifetime = false;

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// 自定义授权策略
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(nameof(Roles.ROOT), policyBuilder =>
            {
                policyBuilder.RequireRole(nameof(Roles.ROOT));
            });

            options.AddPolicy(nameof(Roles.ADMIN), policyBuilder =>
            {
                policyBuilder.RequireRole(nameof(Roles.ROOT), nameof(Roles.ADMIN));
            });

            options.AddPolicy(nameof(Roles.NORMAL), policyBuilder =>
            {
                policyBuilder.RequireRole(nameof(Roles.ROOT), nameof(Roles.ADMIN), nameof(Roles.NORMAL));
            });
        });

        return services;
    }
}


public enum Roles
{
    ROOT,
    ADMIN,
    NORMAL
}

