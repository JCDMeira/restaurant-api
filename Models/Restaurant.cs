using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.Models;

public class Restaurant : MongoBaseEntity
{
    // tem como caracterizar como unique pelo mongo ?
    [BsonElement("Name")]
    [MinLength(10)]
    public string Name { get; set; } = null!;

    public string OpenHour { get; set; } = null!;

    public string CloseHour { get; set; } = null!;
}