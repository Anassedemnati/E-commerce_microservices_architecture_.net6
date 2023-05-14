using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.GRPC.Protos;
using MassTransit;
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

var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

builder.Services.AddScoped<IBasketRepository, BasketRepository>();

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o =>
{
    o.Address = new Uri(configuration["GrpcSettings:DiscountUrl"]);
});

builder.Services.AddScoped<DiscountGrpcService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//AddMassTransit alredy have retryMecanisme 
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((_, cfg) => {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
    
});

//builder.Services.AddMassTransitHostedService(); we can ignore it in the new ver Of MassTransit



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
