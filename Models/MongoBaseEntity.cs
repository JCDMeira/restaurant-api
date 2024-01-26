using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RestaurantApi.Models;

public class MongoBaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public DateTime? CreatedTime { get; } = DateTime.Now!;

    public DateTime? UpdatedTime { get; set; } = DateTime.Now!;
}


