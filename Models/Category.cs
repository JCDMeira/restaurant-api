namespace RestaurantApi.Models;

    public class Category : MongoBaseEntity
    {
    // tem como caracterizar como unique pelo mongo ?
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
}

