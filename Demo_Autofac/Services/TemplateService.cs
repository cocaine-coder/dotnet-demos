
using Autofac.Extras.DynamicProxy;
using Demo_Autofac.Aop;

namespace Demo_Autofac.Services;

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
