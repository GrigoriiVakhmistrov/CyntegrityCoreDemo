using Core.Models;
using Core.MongoDB;
using Core.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pipeline.MongoDB {
    public class PipelineMongoDbRepository : MongoDbRepository, IPipelineRepository {
        readonly IMongoCollection<PipelineMongoDbModel> PipelineCollection;
        public PipelineMongoDbRepository(IConfiguration configuration) : base(configuration["ConnectionStrings:MongoDb"]) {
            PipelineCollection = Database.GetCollection<PipelineMongoDbModel>("pipelines");
        }
        public virtual async Task<IEnumerable<IPipeline>> GetAllAsync() {
            var builder = new FilterDefinitionBuilder<PipelineMongoDbModel>();
            var filter = builder.Empty;
            var pipelines = await PipelineCollection.Find(filter).SortByDescending(e => e.CreatedAt).ToListAsync();
            return pipelines;
        }
        public virtual async Task<IPipeline> GetAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            return await PipelineCollection.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }
        public async Task CreateAsync(IPipeline pipeline) {
            if (pipeline == null)
                return;
            PipelineMongoDbModel dbo = new PipelineMongoDbModel(pipeline);
            await PipelineCollection.InsertOneAsync(dbo);
        }
        public async Task UpdateAsync(IPipeline pipeline) {
            if (pipeline == null)
                return;
            PipelineMongoDbModel dbo = new PipelineMongoDbModel(pipeline);
            await PipelineCollection.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(pipeline.Id)), dbo);
        }
        public async Task RemoveAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return;
            await PipelineCollection.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
    }
}
