using Serilog;
using Serilog.Sinks.Elasticsearch;
using Shopping.Aggregator.Services;

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


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]));
builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]));
builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
