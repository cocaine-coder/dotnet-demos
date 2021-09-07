
using Castle.DynamicProxy;

namespace Demo_Autofac.Aop;

public class CustomAutofacAop : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        {
            Console.WriteLine("方法执行前...");
        }

        invocation.Proceed();

        {
            Console.WriteLine("方法执行后...");
        }
    }
}
