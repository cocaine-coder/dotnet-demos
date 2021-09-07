
namespace Demo_Autofac.Services;

/// <summary>
/// 记录依赖注入创建实例的个数
/// 需要注入为单例模式
/// </summary>
public interface IResolveCountService
{
    void Count(Type type);
}

public class ResolveCountService : IResolveCountService
{
    private readonly ILogger logger;
    private Dictionary<Type, int> _countDic = new();

    public ResolveCountService(ILogger<ResolveCountService> logger)
    {
        this.logger = logger;
    }

    public void Count(Type type)
    {
        if (_countDic.TryGetValue(type, out var count))
        {
            count += 1;
            _countDic[type] = count;
        }
        else
        {
            count = 1;
            _countDic.Add(type, count);
        }

       

        logger.LogInformation($"{type.Name} is resolved {count}");
    }
}
