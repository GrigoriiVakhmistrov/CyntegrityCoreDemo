using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipelineTask.MongoDB {
    public class IdentityPipelineTaskMongoDbService : PipelineTaskMongoDbRepository {
        private readonly UserManager<IdentityUser> _userManager;
        public IdentityPipelineTaskMongoDbService(UserManager<IdentityUser> manager, IConfiguration configuration)
            : base(configuration) {
            _userManager = manager;
        }
        public async override Task<IEnumerable<IPipelineTask>> GetAllAsync() {
            var tasks = await base.GetAllAsync();
            foreach (var task in tasks)
                task.UserName = await GetUserNameAsync(task.UserId);
            return tasks;
        }
        public async override Task<IPipelineTask> GetAsync(string id) {
            var task = await base.GetAsync(id);
            if (task != null)
                task.UserName = await GetUserNameAsync(task.UserId);
            return task;
        }
        async Task<string> GetUserNameAsync(string userId) {
            if (string.IsNullOrWhiteSpace(userId))
                return null;
            return (await _userManager.FindByIdAsync(userId)).UserName;
        }
    }
}
