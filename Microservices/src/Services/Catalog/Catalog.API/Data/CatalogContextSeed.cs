using Catalog.API.Entitys;
using MongoDB.Driver;

namespace Catalog.API.Data;

public class CatalogContextSeed
{
    public static void SeedData(IMongoCollection<Product> productCollection)
    {
        var isExist = productCollection.Find(p => true).Any();
        if (!isExist)
        {
            productCollection.InsertManyAsync(GetPreconfiguredProducts());
        }
    }

    private static IEnumerable<Product> GetPreconfiguredProducts()
    {
        return new List<Product>()
        {
            new Product()
            {
                Name = "IPhone X",
                Summary = "This is a summary",
                Description = "This is a description",
                ImageFile = "product-1.png",
                Price = 950.00M,
                Category = "Smart Phone"
            },
            new Product()
            {
                Name = "Samsung 10",
                Summary = "This is a summary",
                Description = "This is a description",
                ImageFile = "product-2.png",
                Price = 850.00M,
                Category = "Smart Phone"
            },
            new Product()
            {
                Name = "Huawei Plus",
                Summary = "This is a summary",
                Description = "This is a description",
                ImageFile = "product-3.png",
                Price = 750.00M,
                Category = "Smart Phone"
            },
            new Product()
            {
                Name = "Xiaomi Redmi",
                Summary = "This is a summary",
                Description = "This is a description",
                ImageFile = "product-4.png",
                Price = 650.00M,
                Category = "Smart Phone"
            },
            new Product()
            {
                Name = "Xiaomi Mi 10",
                Summary = "This is a summary",
                Description = "This is a description",
                ImageFile = "product-5.png",
                Price = 550.00M,
                Category = "Smart Phone"
            },
            new Product()
            {
                Name = "Xiaomi Mi 10 Pro",
                Summary = "This is a summary",
                Description = "This is a description",
                ImageFile = "product-6.png",
                Price = 450.00M,
                Category = "Smart Phone"
            },
            new Product()
            {
                Name = "Xiaomi Mi 10 Ultra",
                Summary = "This is a summary",
                Description = "This is a description",
                ImageFile = "product-7.png",
                Price = 350.00M,
                Category = "Smart Phone"
            },
            new Product()
            {
                Name = "Xiaomi Mi 10 Ultra Pro",
                Summary = "This is a summary",
                Description = "This is a description",
                ImageFile = "product-8.png",
                Price = 250.00M,
                Category = "Smart"

            }
        };

    }
}
   