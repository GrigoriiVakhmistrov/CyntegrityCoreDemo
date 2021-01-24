using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pipeline.MongoDB {
    public class IdentityPipelineMongoDbRepository : PipelineMongoDbRepository {
        private readonly UserManager<IdentityUser> _userManager;
        public IdentityPipelineMongoDbRepository(UserManager<IdentityUser> manager, IConfiguration configuration)
            : base(configuration) {
            _userManager = manager;
        }
        public async override Task<IEnumerable<IPipeline>> GetAllAsync() {
            var pipelines = await base.GetAllAsync();
            foreach (var pipeline in pipelines)
                pipeline.UserName = await GetUserNameAsync(pipeline.UserId);
            return pipelines;
        }
        public async override Task<IPipeline> GetAsync(string id) {
            var pipeline = await base.GetAsync(id);
            if (pipeline != null)
                pipeline.UserName = await GetUserNameAsync(pipeline.UserId);
            return pipeline;
        }
        async Task<string> GetUserNameAsync(string userId) {
            if (string.IsNullOrWhiteSpace(userId))
                return null;
            return (await _userManager.FindByIdAsync(userId)).UserName;
        }
    }
}
