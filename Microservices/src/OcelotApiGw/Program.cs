using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, configuration) => {
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


IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true)
                            .Build();

builder.Services.AddOcelot(configuration);


builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));


});



var app = builder.Build();

await app.UseOcelot();

app.MapGet("/", () => "Hello World!");

app.Run();
