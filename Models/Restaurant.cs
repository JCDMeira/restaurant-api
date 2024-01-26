using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.Models;

public class Restaurant : MongoBaseEntity
{
    [BsonElement("Name")]
    [MinLength(10)]
    public string Name { get; set; } = null!;

    [Range(0,24)]
    public int OpenHour { get; set; }

    [Range(0, 24)]
    public int CloseHour { get; set; } 
}