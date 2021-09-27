using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Demo_EFCore
{
    internal class AppDbContext : DbContext
    {
        private int? tenantId;

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Rider> Riders { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }

        public AppDbContext(int? tenantId = null)
        {
            this.tenantId = tenantId;
        }

        public void SetTenantId(int tenantId)=> this.tenantId = tenantId;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseInMemoryDatabase("test");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ConfigInherit()
                .ConfigValueConverter()
                .ConfigFilter(tenantId);

            base.OnModelCreating(modelBuilder);
        }
    }

    static class ModelBuilderExtension
    {
        /// <summary>
        /// 配置继承
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <returns></returns>
        public static ModelBuilder ConfigInherit(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>()
               .HasDiscriminator<CartType>("cart_type")
               .HasValue<Cart>(CartType.Cart)
               .HasValue<Coupe>(CartType.Coupe)
               .HasValue<Motorcycle>(CartType.Motorcycle)
               .HasValue<Bicycle>(CartType.Bicycle);

            return modelBuilder;
        }

        /// <summary>
        /// 配置值转换
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <returns></returns>
        public static ModelBuilder ConfigValueConverter(this ModelBuilder modelBuilder)
        {
            //这里使用了内置的转换器
            modelBuilder.Entity<Rider>()
                .Property(x => x.Mount)
                .HasConversion<int>();

            //显示创建转换器
            //modelBuilder
            //    .Entity<Rider>()
            //    .Property(e => e.Mount)
            //    .HasConversion(
            //        v => v.ToString(),
            //        v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));

            //定义转换后使用
            //var converter = new ValueConverter<EquineBeast, string>(
            //    v => v.ToString(),
            //    v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));
            //modelBuilder
            //    .Entity<Rider>()
            //    .Property(e => e.Mount)
            //    .HasConversion(converter);

            return modelBuilder;
        }

        /// <summary>
        /// 配置筛选
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static ModelBuilder ConfigFilter(this ModelBuilder modelBuilder,int? tenantId)
        {
            modelBuilder.Entity<Blog>().HasQueryFilter(b => EF.Property<int>(b, "TenantId") == tenantId);
            modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);

            return modelBuilder;
        }
    }

    #region 继承

    internal enum CartType
    {
        Cart,
        Coupe,
        Motorcycle,
        Bicycle
    }

    internal class Cart
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }


    internal class Coupe : Cart
    {
        public int TypeOfOil { get; set; }

        public int TypeOfche { get; set; }
    }

    internal class Motorcycle : Cart
    {
        public int TypeOfOil { get; set; }

        public int NumOfTire { get; set; }
    }

    internal class Bicycle : Cart
    {
        public int TypeOfPedal { get; set; }
    }
    #endregion

    #region 值转换
    public enum EquineBeast
    {
        Donkey,
        Mule,
        Horse,
        Unicorn
    }

    internal class Rider
    {
        public int Id { get; set; }

        //可以在这里进行值转换
        //[Column(TypeName = "nvarchar(24)")]
        public EquineBeast Mount { get; set; }
    }
    #endregion

    #region 全局查询筛选器

    public class Blog
    {
      

        public Blog(int tenantId , int blogId,string name,string url)
        {
            this.TenantId = tenantId;
            this.BlogId = blogId;
            this.Name = name;
            this.Url = url;
        }

        public int TenantId { get; set; }

        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }

        public Blog Blog { get; set; }
    } 

    #endregion

}
