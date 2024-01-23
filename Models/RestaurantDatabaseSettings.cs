namespace RestaurantApi.Models;

public class RestaurantDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string RestaurantsCollectionName { get; set; } = null!;
}