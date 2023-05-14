using EventBus.Messages.Common;
using MassTransit;
using Ordering.API;
using Ordering.API.EventBusConsumer;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Serilog.Sinks.Elasticsearch;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context,configuration)=> {
    configuration.Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(
        new ElasticsearchSinkOptions(
            new Uri(builder.Configuration["ElasticConfiguration:Uri"]))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"{builder.Configuration["ApplicationName"]}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
            NumberOfReplicas = 1,
            NumberOfShards = 2,

        })
    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
    .ReadFrom.Configuration(context.Configuration);
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<BasketCheckoutConsumer>();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>();  
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);



        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
        });
    });

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


app.MigrateDatabase<OrderContext>(
    (context,services) =>
    {
        var logger = services.GetService<ILogger<OrderContextSeed>>();
        OrderContextSeed
            .SeedAsync(context, logger)
            .Wait();
    });

app.Run();
