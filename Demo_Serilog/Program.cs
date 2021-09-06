using Serilog;

var builder = WebApplication.CreateBuilder(args);

//´ÓÅäÖÃÎÄµµÖÐÅäÖÃserilog
builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
});

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Demo_Serilog", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo_Serilog v1"));
}

app.UseSerilogRequestLogging();

app.MapGet("logname", (string name, Serilog.ILogger logger) =>
{
    logger.Debug(name);
    logger.Information(name);
    logger.Warning(name);
    logger.Error(name);
    logger.Fatal(name);

    return Results.Ok(name);
});

app.Run();
