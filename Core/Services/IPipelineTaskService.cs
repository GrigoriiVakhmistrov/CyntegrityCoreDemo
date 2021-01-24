using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services {
    public interface IPipelineTaskService {
        public Task<IEnumerable<IPipelineTask>> GetAllAsync();
        public Task<IPipelineTask> GetAsync(string id);
        public Task CreateAsync(IPipelineTask task, string userId);
        public Task UpdateAsync(IPipelineTask task);
        public Task RemoveAsync(string id);
    }
}
