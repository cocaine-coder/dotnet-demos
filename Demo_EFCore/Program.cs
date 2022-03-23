// See https://aka.ms/new-console-template for more information
using Demo_EFCore;
using System.Text.Json;

using var dbContext = new AppDbContext(1);

dbContext.Database.EnsureDeleted();

if (dbContext.Database.EnsureCreated())
{
    var blog1 = new Blog(1, 1, "test", "http://xxx.com");
    blog1.Posts = new List<Post>
    {
         new Post()
         {
              Content = "this is a test post",
              Id = 1,
              Title = "test",
              IsDeleted = false,
         },
         new Post()
         {
              Content = "this is a test post",
              Id = 2,
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
              Id = 3,
              Title = "test",
              IsDeleted = false,
         },
         new Post()
         {
              Content = "this is a test post",
              Id = 4,
              Title = "test",
              IsDeleted = true,
         },
    };

    dbContext.Blogs.AddRange(blog1, blog2);


    dbContext.Riders.Add(new Rider()
    {
        Id = 1,
        Mount = EquineBeast.Unicorn
    });

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

    dbContext.Websites.AddRange(new Website
    {
        Id = 1,
        Name = "baidu"
    }, new Website
    {
        Id = 2,
        Name = "sougou"
    }, new Website
    {
        Id = 3,
        Name = "bing"
    }, new Website
    {
        Id = 4,
        Name = "google"
    });

    dbContext.AccessLogs.AddRange(
        new AccessLog
        {
            Id = 1,
            Count = 100,
            Date = new DateTime(2016, 05, 10),
            SiteId = 1,
        }, new AccessLog
        {
            Id = 2,
            Count = 230,
            Date = new DateTime(2016, 05, 11),
            SiteId = 1,
        }, new AccessLog
        {
            Id = 3,
            Count = 45,
            Date = new DateTime(2016, 05, 10),
            SiteId = 2,
        }, new AccessLog
        {
            Id = 4,
            Count = 60,
            Date = new DateTime(2016, 05, 11),
            SiteId = 2,
        }, new AccessLog
        {
            Id = 5,
            Count = 300,
            Date = new DateTime(2016, 05, 10),
            SiteId = 3,
        });


    dbContext.SaveChanges();
}

//Test_Inherit();
//Test_ValueConversion();
//Test_Filter();

Console.WriteLine(new String('\n', 4) + new String('>', 20) + "数据库初始化完成" + new String('<', 20) + new string('\n', 2));

Test_Join();

/// <summary>
/// 全局筛选
/// </summary>
void Test_Filter()
{
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
    Console.WriteLine(JsonSerializer.Serialize(dbContext.Riders.ToList()));
}

/// <summary>
/// 测试继承
/// </summary>
void Test_Inherit()
{
    var carts = dbContext.Carts.ToList();
    var coupes = dbContext.Set<Coupe>().ToList();
    var motorcycles = dbContext.Set<Motorcycle>().ToList();
    var bicycles = dbContext.Set<Bicycle>().ToList();

    Console.WriteLine(JsonSerializer.Serialize(coupes));
    Console.WriteLine(JsonSerializer.Serialize(motorcycles));
    Console.WriteLine(JsonSerializer.Serialize(bicycles));
    Console.WriteLine(JsonSerializer.Serialize(carts));
}

void Test_Join()
{
    Console.WriteLine("inner join");
    var innerjoin = dbContext.Websites
        .Join(dbContext.AccessLogs, x => x.Id, x => x.SiteId, (website, log) => new { website, log })
        .ToList();
    innerjoin.ForEach(x => Console.Write(x.website.Name + " : " + x.log.Count + "  |  "));

    Console.WriteLine("\n\n");
    Console.WriteLine("inner join");
    var anotherInnerjoin = (from website in dbContext.Websites
                            join log in dbContext.AccessLogs
                            on website.Id equals log.SiteId
                            select new
                            {
                                website,
                                log
                            }).ToList();
    anotherInnerjoin.ForEach(x => Console.Write(x.website.Name + " : " + x.log.Count + "  |  "));

    Console.WriteLine("\n\n");
    Console.WriteLine("left join");

    var leftJoin = (from website in dbContext.Websites
                    join accesslog in dbContext.AccessLogs
                    on website.Id equals accesslog.SiteId
                    into logs
                    from log in logs.DefaultIfEmpty()
                    select new
                    {
                        website,
                        log
                    }).ToList();

    leftJoin.ForEach(x => Console.Write(x.website.Name + " : " + x.log?.Count + "  |  "));
}
