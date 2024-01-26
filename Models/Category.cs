namespace RestaurantApi.Models;

    public class Category : MongoBaseEntity
    {
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

