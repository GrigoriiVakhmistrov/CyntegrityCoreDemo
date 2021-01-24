using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories {
    public interface IPipelineRepository {
        public Task<IEnumerable<IPipeline>> GetAllAsync();
        public Task<IPipeline> GetAsync(string id);
        public Task CreateAsync(IPipeline pipeline);
        public Task UpdateAsync(IPipeline pipeline);
        public Task RemoveAsync(string id);
    }

}
