
using Demo_Autofac.Services;

namespace Demo_Autofac.Repositories;
public interface IInUseRepository
{
}

public class InUseRepository: IInUseRepository
{
    public InUseRepository(IResolveCountService resolveCountService)
    {
        resolveCountService.Count(GetType());
    }
}
