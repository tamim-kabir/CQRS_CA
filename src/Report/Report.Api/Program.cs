using Api.Middleware;
using Application.Products.Commands.CreateProduct;
using Carter;
using Domain.Shared;
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
               .AddClasses((ts) =>
               {
                   ts.NotInNamespaceOf(typeof(Result));
               }, false)
               .AsImplementedInterfaces()
               .WithScopedLifetime());

builder.Services.DatabaseConfiguration(builder.Configuration);

builder.Services.AddControllers()
                .AddApplicationPart(Presentation.AssemblyReference.Assembly);
var AppAssebbly = Application.AssemblyReference.Assembly;
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(AppAssebbly));

builder.Services.AddValidatorsFromAssembly(AppAssebbly);

builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();

    config.AddConsumers(AppAssebbly);
    config.AddSagaStateMachines(AppAssebbly);
    config.AddSagas(AppAssebbly);
    config.AddActivities(AppAssebbly);

    config.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["MessageBroker:Host"], "/", host =>

        //configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), host =>

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
