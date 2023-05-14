using Discount.API.Repositories;
using Discount.API.Extension;
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


// Add services to the container.

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Discount.API", Version = "v1" });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Discount.API v1");
    });
}

app.MigrateDatabase<Program>();

app.UseAuthorization();

app.MapControllers();

app.Run();
