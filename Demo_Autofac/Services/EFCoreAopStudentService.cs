
using Demo_Autofac.Aop;
using Microsoft.EntityFrameworkCore;

namespace Demo_Autofac.Services;

/// <summary>
/// student service
/// </summary>
public interface IEFCoreAopStudentService
{
    Task CreateAsync(Student student);

    Task UpdateAsync(int id ,string name);

    Task DeleteAsync(int id);

    Task<Student> GetAsync(int id);

    Task<IEnumerable<Student>> GetAsync();
}

public class EFCoreAopStudentService : IEFCoreAopStudentService
{
    private readonly AppDbContext appDbContext;

    public EFCoreAopStudentService(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    [UnitOfWork]
    public async Task CreateAsync(Student student)
    {
        if (student.Id < 1 || await GetAsync(student.Id) != null)
            return;

        appDbContext.Add(student);
    }

    [UnitOfWork]
    public async Task DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity != null)
            appDbContext.Remove(entity);
    }

    [UnitOfWork(true)]
    public async Task<Student> GetAsync(int id)
    {
        return await appDbContext.Students.FindAsync(id);
    }

    [UnitOfWork(true)]
    public async Task<IEnumerable<Student>> GetAsync()
    {
        return await appDbContext.Students.ToListAsync();
    }

    [UnitOfWork]
    public async Task UpdateAsync(int id ,string name)
    {
        var entity = await GetAsync(id);
        if(entity != null)
            entity.Name = name;
    }
}
