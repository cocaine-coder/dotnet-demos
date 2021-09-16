using Demo_AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIpRateLimit(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Demo_AspNetCoreRateLimit", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo_AspNetCoreRateLimit v1"));
}

if (builder.Configuration.GetValue("AspNetCoreRateLimit:InUse", false))
    app.UseMiddleware<IPLimitMiddleware>();

/// <summary>
/// 全局限流，1s上限两次
/// </summary>
app.MapGet("api/limite_1", () =>
{
    return Results.Ok(DateTime.Now);
});

/// <summary>
/// 这是一个渲染ui的接口，在限流配置中不做限制
/// </summary>
app.MapGet("ui/limite_2", () =>
{
    return Results.Text(@$"
     <html>
        <header>
            <title>limite</title>
        </header>
        <body>
            <p>this is a limite test</p>
        </body>
     </html>
    ","text/html");
});

app.Run();
