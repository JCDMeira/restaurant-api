﻿using RestaurantApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace RestaurantApi.Services;

public class RestaurantService
{
    private readonly IMongoCollection<Restaurant> _restaurantsCollection;

    public RestaurantService(
        IOptions<RestaurantDatabaseSettings> restaurantDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            restaurantDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            restaurantDatabaseSettings.Value.DatabaseName);

        _restaurantsCollection = mongoDatabase.GetCollection<Restaurant>(
            restaurantDatabaseSettings.Value.RestaurantsCollectionName);
    }

    public async Task<List<Restaurant>> GetAsync() =>
        await _restaurantsCollection.Find(_ => true).ToListAsync();

    public async Task<Restaurant?> GetAsync(string id) =>
        await _restaurantsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Restaurant newBook) =>
        await _restaurantsCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, Restaurant updatedBook) =>
        await _restaurantsCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _restaurantsCollection.DeleteOneAsync(x => x.Id == id);
}