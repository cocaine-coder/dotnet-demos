
namespace Demo_Autofac.Services;

public interface ITemplateService<TKey, TEntity> 
{
    void GetNode(TKey key);
}

public class TemplateService<TKey, TEntity> : ITemplateService<TKey, TEntity>
{
    private readonly ILogger logger;

    public TemplateService(ILogger<TemplateService<TKey, TEntity>> logger)
    {
        this.logger = logger;
    }

    public void GetNode(TKey key)
    {
        logger.LogInformation($"key type is {key?.GetType().Name} ; entity type is {typeof(TEntity).Name}");
    }
}
