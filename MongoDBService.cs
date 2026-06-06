using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using real_time_chat.Models;

namespace real_time_chat.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDbConnection"));
            _database = client.GetDatabase("ChatAppDB");
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }
}