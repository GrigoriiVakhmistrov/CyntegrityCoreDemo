using MongoDB.Driver;

namespace Core.MongoDB {
    public class MongoDbRepository {
        public IMongoDatabase Database;
        public MongoDbRepository(string connectionString) {
            var connection = new MongoUrlBuilder(connectionString);
            MongoClient client = new MongoClient(connectionString);
            Database = client.GetDatabase(connection.DatabaseName);
        }
    }
}
