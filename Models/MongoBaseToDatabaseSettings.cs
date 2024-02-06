namespace restaurant_api.Models
{
    public class MongoBaseToDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

    }
}

//acho que só precisa de uma DatabaseSettings, se a chave para CollectionName for a mesma para ambos, diferenciando apenas o valor