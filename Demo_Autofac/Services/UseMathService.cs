
using Demo_Autofac.Repositories;

namespace Demo_Autofac.Services;

/// <summary>
/// 依赖IMathService，测试MathService创建实例的方式
/// </summary>
public interface IUseMathService
{
    double Add(double x1, double x2);
}


public class UseMathService : IUseMathService
{
    private readonly IMathService mathService;

    public UseMathService(IMathService mathService, IResolveCountService resolveCountService)
    {
        this.mathService = mathService ?? throw new ArgumentNullException(nameof(mathService));
        resolveCountService.Count(GetType());
    }

    public double Add(double x1, double x2)
    {
        return mathService.Add(x1, x2);
    }
}
