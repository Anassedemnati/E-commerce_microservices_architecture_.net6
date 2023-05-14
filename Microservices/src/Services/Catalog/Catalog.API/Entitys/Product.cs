using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.API.Entitys;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonElement("name")]
    public string? Name { get; set; }
    [BsonElement("summary")]
    public string? Summary { get; set; }
    [BsonElement("description")]
    public string? Description { get; set; }
    [BsonElement("imageFile")]
    public string? ImageFile { get; set; }
    [BsonElement("price")]
    public decimal Price { get; set; }
    [BsonElement("category")]
    public string? Category { get; set; }

}
