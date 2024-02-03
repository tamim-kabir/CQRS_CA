using Api.Middleware;
using Carter;
using FluentValidation;
using MassTransit;
using Persistence.Configarations;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services
       .Scan(selector =>
       selector.FromAssemblies(Infrastructure.AssemblyReference.Assembly, Persistence.AssemblyReference.Assembly)
               .AddClasses(false)
               .AsImplementedInterfaces()
               .WithScopedLifetime());

builder.Services.DatabaseConfiguration(builder.Configuration);

builder.Services.AddControllers()
                .AddApplicationPart(Presentation.AssemblyReference.Assembly);
var AppAssembly = Application.AssemblyReference.Assembly;
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(AppAssembly));

builder.Services.AddValidatorsFromAssembly(AppAssembly);

//builder.Services.AddMassTransit(config =>
//{
//    config.SetKebabCaseEndpointNameFormatter();

//    config.AddConsumers(AppAssembly);
//    //config.AddSagaStateMachines(AppAssembly);
//    //config.AddSagas(AppAssembly);
//    //config.AddActivities(AppAssembly);

//    config.UsingRabbitMq((context, configurator) =>
//    {
//#if DEBUG
//        configurator.Host(builder.Configuration["MessageBroker:Host"], "/", host =>
//#else
//        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), host =>
//#endif
//        {
//            host.Username(builder.Configuration["MessageBroker:Username"]);
//            host.Password(builder.Configuration["MessageBroker:Password"]);
//        });
//        configurator.ConfigureEndpoints(context);
//    });
//});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<ExceptionHandelingMiddleware>();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//if (!app.Environment.WebRootPath.IsPresent())
//{
//    app.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
//}
app.UseMiddleware<ExceptionHandelingMiddleware>();
app.Services.ConfigureServices();
app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();//Map Minimal api endpoint Using ICarterMoule
app.MapControllers();

app.Run();
