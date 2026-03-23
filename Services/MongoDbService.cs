using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using axiosTest.Models;

namespace axiosTest.Services {
  public class MongoDbService
  {
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoDbService(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        // var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
	var database = mongoClient.GetDatabase("testForDatabase");
        _collection = database.GetCollection<BsonDocument>("testForMongoDbCollection");
    }

    public async Task<List<BsonDocument>> GetAllAsync()
    {
        return await _collection.Find(new MongoDB.Bson.BsonDocument()).ToListAsync();
    }
  }
}
