
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Demo_Autofac.Aop;

/// <summary>
/// efcore uow
/// </summary>
public interface IUnitOfWork : IDisposable
{
    DbContext GetDbContext();

    IDbContextTransaction BeginTransaction();

    Task<IDbContextTransaction> BeginTransactionAsync();

    int SaveChanges();

    Task<int> SaveChangesAsync();

    void Complete(IDbContextTransaction transaction);

    Task CompleteAsync(IDbContextTransaction transaction);

    void RollBackChanges(IDbContextTransaction transaction);

    Task RollBackChangesAsync(IDbContextTransaction transaction);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext appDbContext;

    public UnitOfWork(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public IDbContextTransaction BeginTransaction()
    {
        return appDbContext.Database.BeginTransaction();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await appDbContext.Database.BeginTransactionAsync();
    }

    public void Complete(IDbContextTransaction transaction)
    {
        appDbContext.SaveChanges();
        transaction.Commit();
    }

    public async Task CompleteAsync(IDbContextTransaction transaction)
    {
        await appDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public void Dispose()
    {
        appDbContext.Dispose();
    }

    public DbContext GetDbContext()
    {
        return appDbContext;
    }

    public void RollBackChanges(IDbContextTransaction transaction)
    {
        transaction.Rollback();
    }

    public async Task RollBackChangesAsync(IDbContextTransaction transaction)
    {
        await transaction.RollbackAsync();
    }

    public int SaveChanges()
    {
        return appDbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await appDbContext.SaveChangesAsync();
    }
}