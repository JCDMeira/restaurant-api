namespace restaurant_api.Models
{
    public class CategoriesDatabaseSettings : MongoBaseToDatabaseSettings
    {
        public string CategoriesCollectionName { get; set; } = null!;
    }
}
