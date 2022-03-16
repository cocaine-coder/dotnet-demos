using Demo_MediatR;
using Demo_MediatR.Notification;
using Demo_MediatR.PipelineBehavior;
using Demo_MediatR.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var services = builder.Services;

services.AddMediatR(typeof(Program));
services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
services.AddSingleton(typeof(DataContext));

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("api/users", async ([FromBody] AddUserRequest addUserRequest,[FromServices] IMediator mediator) =>
{
    var ret = await mediator.Send(addUserRequest);

    if(ret)
        await mediator.Publish(
            new SendMessageNotification { Message = $"×¢²á³É¹¦ : {addUserRequest.Name}" });

    return Results.Ok(ret);
});

app.MapGet("api/users", ([FromServices] DataContext dataContext) =>
{
    return Results.Ok(dataContext.Users);
});

app.Run();