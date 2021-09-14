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

app.MapGet("api/limite_1", () =>
{
    return Results.Ok(DateTime.Now);
});

app.Run();
