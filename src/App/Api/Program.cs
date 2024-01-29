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
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(Application.AssemblyReference.Assembly));

builder.Services.AddValidatorsFromAssembly(Application.AssemblyReference.Assembly);

builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();
    var assembly = Assembly.GetEntryAssembly();
    config.AddConsumers(assembly);
    config.AddSagaStateMachines(assembly);
    config.AddSagas(assembly);
    config.AddActivities(assembly);

    config.UsingRabbitMq((context, configurator) =>
    {
#if DEBUG
        configurator.Host(builder.Configuration["MessageBroker:Host"], "/", host =>
#else
        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), host =>
#endif
        {
            host.Username(builder.Configuration["MessageBroker:Username"]);
            host.Password(builder.Configuration["MessageBroker:Password"]);
        });
        configurator.ConfigureEndpoints(context);
    });
});
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
app.Services.ConfigureServices();
app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
//Custom Middleware for exception handalling
app.UseMiddleware<ExceptionHandelingMiddleware>();

app.MapCarter();//Map Minimal api endpoint Using ICarterMoule
app.MapControllers();

app.Run();
