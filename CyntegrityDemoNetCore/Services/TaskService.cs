using CyntegrityDemoNetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyntegrityDemoNetCore.Services {
    public class PipelineTaskService : MongoDbService {
        readonly IMongoCollection<PipelineTask> TaskCollection;
        private readonly PipelineService _pipelineDb;
        private readonly UserManager<IdentityUser> _userManager;
        public PipelineTaskService(UserManager<IdentityUser> manager, IConfiguration configuration, PipelineService pipelineContext)
            : base(configuration["ConnectionStrings:MongoDb"]) {
            _userManager = manager;
            _pipelineDb = pipelineContext;
            TaskCollection = Database.GetCollection<PipelineTask>("tasks");
        }
        public async Task<IEnumerable<PipelineTask>> GetTasks() {
            var builder = new FilterDefinitionBuilder<PipelineTask>();
            var filter = builder.Empty;
            var tasks = await TaskCollection.Find(filter).SortByDescending(e => e.CreatedAt).ToListAsync();
            foreach(var task in tasks)
                task.UserName = await GetUserNameAsync(task.UserId);
            return tasks;
        }
        public async Task<PipelineTask> Get(string id) {
            var task = await TaskCollection.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
            task.UserName = await GetUserNameAsync(task.UserId);
            return task;
        }
        public async Task Create(PipelineTask task) {
            task.CreatedAt = DateTime.UtcNow;
            await TaskCollection.InsertOneAsync(task);
        }
        public async Task Update(PipelineTask p) {
            await TaskCollection.ReplaceOneAsync(new BsonDocument("_id", p.Id), p);
        }
        public async Task Remove(string id) {
            var filter = new BsonDocument("tasks", new ObjectId(id));
            var affectedPipelines = await _pipelineDb.Find(filter);
            foreach (var pipeline in affectedPipelines) {
                pipeline.TaskIds = pipeline.TaskIds.Except(new List<ObjectId>() { new ObjectId(id) }).ToList();
                await _pipelineDb.Update(pipeline);
            }
            await TaskCollection.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
        public async Task<string> GetUserNameAsync(string userId) {
            return (await _userManager.FindByIdAsync(userId)).UserName;
        }
    }
}
