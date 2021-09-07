
using Castle.Core.Internal;
using Castle.DynamicProxy;
using System.Reflection;

namespace Demo_Autofac.Aop;

/// <summary>
/// 工作单元Attribute
/// 控制工作单元Aop是否实现
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
public class UnitOfWorkAttribute : Attribute
{
    public UnitOfWorkAttribute(bool isDisabled = false)
    {
        IsDisabled = isDisabled;
    }
    public bool IsDisabled { get; set; }
}


/// <summary>
/// 实现一个基于efcore的工作单元
/// 目标：service执行方法时，通过配置自动实现SaveChange() 或 SaveChangeAsync()
/// 根据微软的官网，SaveChange 自动实现事务
/// </summary>
public class UnitOfWorkInterceptor : IInterceptor
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<UnitOfWorkInterceptor> logger;

    public UnitOfWorkInterceptor(IUnitOfWork unitOfWork,ILogger<UnitOfWorkInterceptor> logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public void Intercept(IInvocation invocation)
    {
        MethodInfo method;

        try
        {
            method = invocation.MethodInvocationTarget;
        }
        catch
        {
            method = invocation.GetConcreteMethod();
        }

        var unitOfWorkAttr = method.GetAttribute<UnitOfWorkAttribute>();

        if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
        {
            invocation.Proceed();
            return;
        }

        ActionUow(invocation);
    }

    private void ActionUow(IInvocation invocation)
    {
        //判断是否为异步
        if (IsAsync(invocation.Method))
        {
            invocation.Proceed();

            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                    (Task)invocation.ReturnValue,
                    async () => await unitOfWork.SaveChangesAsync(),
                    exception => { if (exception != null) logger.LogError(exception, ""); }
                );
            }
            else //Task<TResult>
            {
                invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    async () => await unitOfWork.SaveChangesAsync(),
                    exception => { if (exception != null) logger.LogError(exception, ""); }
                );
            }
        }
        else
        {
            invocation.Proceed();
            unitOfWork.SaveChanges();
            return;
        }
    }

    private static bool IsAsync(MethodInfo method)
    {
        return (
            method.ReturnType == typeof(Task) ||
            (method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        );
    }
}
