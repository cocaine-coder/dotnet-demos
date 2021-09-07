using Demo_Autofac.Repositories;

namespace Demo_Autofac.Services;

public interface IMathService
{
    double Add(double x1, double x2);
}

public class MathService : IMathService
{
    private readonly IInUseRepository inUseRepository;

    public MathService(IResolveCountService resolveCountService, IInUseRepository inUseRepository)
    {
        resolveCountService.Count(GetType());
        this.inUseRepository = inUseRepository;
    }

    public double Add(double x1, double x2)
    {
        return x1 + x2;
    }
}
