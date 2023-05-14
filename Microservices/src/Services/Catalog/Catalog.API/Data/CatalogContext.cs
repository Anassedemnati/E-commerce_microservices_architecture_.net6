using Catalog.API.Entitys;
using MongoDB.Driver;

namespace Catalog.API.Data;

public class CatalogContext : ICatalogContext
{

    public CatalogContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetValue<string>("DatabaseSettigs:ConnectionString"));
        var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettigs:DatabaseName"));

        Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettigs:CollectionName"));
        CatalogContextSeed.SeedData(Products);
    }
    public IMongoCollection<Product> Products { get; }
}
