using restaurant_api.Models;

namespace RestaurantApi.Models;

public class RestaurantDatabaseSettings : MongoBaseToDatabaseSettings
{
    public string RestaurantsCollectionName { get; set; } = null!;
}