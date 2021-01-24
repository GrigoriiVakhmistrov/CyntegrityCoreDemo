using Core.Models;
using Core.MongoDB;
using Core.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipelineTask.MongoDB {
    public class PipelineTaskMongoDbRepository : MongoDbRepository, IPipelineTaskRepository {
        readonly IMongoCollection<PipelineTaskMongoDbModel> TaskCollection;
        public PipelineTaskMongoDbRepository(IConfiguration configuration) : base(configuration["ConnectionStrings:MongoDb"]) {
            TaskCollection = Database.GetCollection<PipelineTaskMongoDbModel>("tasks");
        }
        public virtual async Task<IEnumerable<IPipelineTask>> GetAllAsync() {
            var builder = new FilterDefinitionBuilder<PipelineTaskMongoDbModel>();
            var filter = builder.Empty;
            return await TaskCollection.Find(filter).SortByDescending(e => e.CreatedAt).ToListAsync();
        }
        public virtual async Task<IPipelineTask> GetAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            return await TaskCollection.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }
        public async Task CreateAsync(IPipelineTask task) {
            if (task == null)
                return;
            PipelineTaskMongoDbModel dbo = new PipelineTaskMongoDbModel(task);
            await TaskCollection.InsertOneAsync(dbo);
        }
        public async Task UpdateAsync(IPipelineTask task) {
            if (task == null)
                return;
            PipelineTaskMongoDbModel dbo = new PipelineTaskMongoDbModel(task);
            await TaskCollection.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(task.Id)), dbo);
        }
        public async Task RemoveAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return;
            await TaskCollection.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
    }
}
