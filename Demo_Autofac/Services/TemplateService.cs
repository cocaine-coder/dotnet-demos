
using Autofac.Extras.DynamicProxy;
using Demo_Autofac.Aop;

namespace Demo_Autofac.Services;

/// <summary>
/// 测试AOP 以及泛型动态注入
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TEntity"></typeparam>
[Intercept(typeof(CustomAutofacAop))]
public interface ITemplateService<TKey, TEntity>
{
    void GetNode(TKey key);

    Task<TKey> GetKeyAsync(TKey key);
}

public class TemplateService<TKey, TEntity> : ITemplateService<TKey, TEntity>
{
    private readonly ILogger logger;

    public TemplateService(ILogger<TemplateService<TKey, TEntity>> logger)
    {
        this.logger = logger;
    }

    public Task<TKey> GetKeyAsync(TKey key)
    {
        logger.LogInformation("GetKeyAsync is actioning");
        return Task.FromResult(key);
    }

    public void GetNode(TKey key)
    {
        logger.LogInformation($"key type is {key?.GetType().Name} ; entity type is {typeof(TEntity).Name}");
    }
}
