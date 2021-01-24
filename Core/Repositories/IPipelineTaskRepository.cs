using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories {
    public interface IPipelineTaskRepository {
        public Task<IEnumerable<IPipelineTask>> GetAllAsync();
        public Task<IPipelineTask> GetAsync(string id);
        public Task CreateAsync(IPipelineTask task);
        public Task UpdateAsync(IPipelineTask task);
        public Task RemoveAsync(string id);
    }
}
