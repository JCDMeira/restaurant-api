using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.Models;

public class Restaurant
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]
    [MinLength(10)]
    public string Name { get; set; } = null!;


    public DateTime? Created_time { get; } = DateTime.Now!;

    public DateTime? Updated_time { get; set; } = DateTime.Now!;
}