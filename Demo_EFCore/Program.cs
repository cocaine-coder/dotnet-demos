// See https://aka.ms/new-console-template for more information
using Demo_EFCore;
using System.Text.Json;

using var dbContext = new AppDbContext(1);

//Test_Inherit();
//Test_ValueConversion();
Test_Filter();

/// <summary>
/// 全局筛选
/// </summary>
void Test_Filter()
{
    var blog1 = new Blog(1, 1, "test", "http://xxx.com");
    blog1.Posts = new List<Post>
    {
         new Post()
         {
              Content = "this is a test post",
              PostId = 1,
              Title = "test",
              IsDeleted = false,
         },
         new Post()
         {
              Content = "this is a test post",
              PostId = 2,
              Title = "test",
              IsDeleted = true,
         },
    };

    var blog2 = new Blog(2, 2, "test", "http://xxx.com");
    blog2.Posts = new List<Post>
    {
         new Post()
         {
              Content = "this is a test post",
              PostId = 3,
              Title = "test",
              IsDeleted = false,
         },
         new Post()
         {
              Content = "this is a test post",
              PostId = 4,
              Title = "test",
              IsDeleted = true,
         },
    };

    dbContext.Blogs.AddRange(blog1,blog2);
    dbContext.SaveChanges();

    var blogs = dbContext.Blogs.ToList();
    Console.WriteLine(blogs.Count);

    var posts = dbContext.Posts.ToList();
    Console.WriteLine(posts.Count);
}

/// <summary>
/// 测试 值转换
/// </summary>
void Test_ValueConversion()
{
    if (dbContext == null) return;

    dbContext.Riders.Add(new Rider()
    {
        Id = 1,
        Mount = EquineBeast.Unicorn
    });

    dbContext.SaveChanges();

    Console.WriteLine(JsonSerializer.Serialize(dbContext.Riders.ToList()));
}

/// <summary>
/// 测试继承
/// </summary>
void Test_Inherit()
{
    if (dbContext == null) return;

    dbContext.Carts.AddRange(

        new Coupe
        {
            Id = 1,
            Name = "丰田86",
            TypeOfche = 1,
            TypeOfOil = 1
        },

        new Motorcycle
        {
            Id = 2,
            Name = "隼",
            TypeOfOil = 2,
            NumOfTire = 2,
        },

        new Bicycle
        {
            Id = 3,
            Name = "凤凰",
            TypeOfPedal = 3,
        }
    );

    dbContext.SaveChanges();

    var carts = dbContext.Carts.ToList();
    var coupes = dbContext.Set<Coupe>().ToList();
    var motorcycles = dbContext.Set<Motorcycle>().ToList();
    var bicycles = dbContext.Set<Bicycle>().ToList();

    Console.WriteLine(JsonSerializer.Serialize(coupes));
    Console.WriteLine(JsonSerializer.Serialize(motorcycles));
    Console.WriteLine(JsonSerializer.Serialize(bicycles));
    Console.WriteLine(JsonSerializer.Serialize(carts));
}
