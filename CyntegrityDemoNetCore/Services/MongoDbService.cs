using MongoDB.Driver;

namespace CyntegrityDemoNetCore.Services {
    public class MongoDbService {
        public IMongoDatabase Database;
        public MongoDbService(string connectionString) {
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);
            Database = client.GetDatabase(connection.DatabaseName);
        }
    }
}
