
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

namespace Demo_AspNetCoreRateLimit;
public static class AspNetCoreRateLimitExtension
{
    public static IServiceCollection AddIpRateLimit(this IServiceCollection services,IConfiguration configuration)
    {
        if(services == null) throw new ArgumentNullException(nameof(services));
        if(configuration == null) throw new ArgumentNullException(nameof(configuration));

        if (!configuration.GetValue("AspNetCoreRateLimit:InUse", false))
            return services;

        services.AddOptions();

        //加载配置
        //从appsettings.json获取相应配置
        services.Configure<IpRateLimitOptions>(configuration.GetSection("AspNetCoreRateLimit:IpRateLimiting"));

        //注入计数器和规则存储
        //使用简单内存方案
        services.AddMemoryCache();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

        //使用分布式缓存
        //services.AddDistributedCache(configuration);
        //services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
        //services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

        //配置（计数器密钥生成器）
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


        return services;
    }

    public static IServiceCollection AddDistributedCache(this IServiceCollection services,IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var connection = configuration["Redis:Configuration"];
        var instanceName = configuration["Redis:InstanceName"];

        if (!string.IsNullOrEmpty(connection))
        {
            var redis = ConnectionMultiplexer.Connect(connection);//建立Redis 连接

            //添加数据保护服务，设置统一应用程序名称，并指定使用Reids存储私钥
            services.AddDataProtection()
                .SetApplicationName(configuration["AppName"])
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

            //添加Redis缓存用于分布式Session
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connection;
                options.InstanceName = instanceName;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }
}
