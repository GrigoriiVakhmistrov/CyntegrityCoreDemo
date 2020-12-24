using CyntegrityDemoNetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyntegrityDemoNetCore.Services {
    public class PipelineService : MongoDbService {
        readonly IMongoCollection<Pipeline> PipelineCollection;
        private readonly UserManager<IdentityUser> _userManager;
        public PipelineService(UserManager<IdentityUser> manager, IConfiguration configuration)
            : base(configuration["ConnectionStrings:MongoDb"]) {
            _userManager = manager;
            PipelineCollection = Database.GetCollection<Pipeline>("pipelines");
        }
        public async Task<IEnumerable<Pipeline>> GetPipelines() {
            var builder = new FilterDefinitionBuilder<Pipeline>();
            var filter = builder.Empty;
            var pipelines = await PipelineCollection.Find(filter).SortByDescending(e => e.CreatedAt).ToListAsync();
            foreach(var pipeline in pipelines)
                pipeline.UserName = await GetUserNameAsync(pipeline.UserId);
            return pipelines;
        }
        public async Task<Pipeline> Get(string id) {
            var pipeline = await PipelineCollection.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
            pipeline.UserName = await GetUserNameAsync(pipeline.UserId);
            return pipeline;
        }
        public async Task Create(Pipeline pipeline) {
            pipeline.CreatedAt = DateTime.UtcNow;
            await PipelineCollection.InsertOneAsync(pipeline);
        }
        public async Task Update(Pipeline p) {
            await PipelineCollection.ReplaceOneAsync(new BsonDocument("_id", p.Id), p);
        }
        public async Task Remove(string id) {
            await PipelineCollection.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
        public async Task<List<Pipeline>> Find(BsonDocument filter) {
            return await PipelineCollection.Find(filter).ToListAsync();
        }
        public async Task<string> GetUserNameAsync(string userId) {
            return (await _userManager.FindByIdAsync(userId)).UserName;
        }
    }
}
