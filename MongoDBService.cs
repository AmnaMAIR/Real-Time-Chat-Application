using MongoDB.Driver;
using real_time_chat.Models;

namespace real_time_chat.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
    }
}
