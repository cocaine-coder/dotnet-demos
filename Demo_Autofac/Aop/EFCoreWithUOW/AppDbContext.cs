
using Microsoft.EntityFrameworkCore;

namespace Demo_Autofac.Aop;
public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {

    }

    public DbSet<Student> Students {  get; set;}
}

public class Student
{
    public int Id {  get; set; }

    public string Name {  get; set; }
}
