using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services {
    public interface IPipelineService {
        public Task<IEnumerable<IPipeline>> GetAllAsync();
        public Task<IPipeline> GetAsync(string id);
        public Task CreateAsync(IPipeline pipeline, string userId);
        public Task UpdateAsync(IPipeline pipeline);
        public Task RemoveAsync(string id);
        public Task<int> CalculateAsync(string id);
        public Task<int> RunAsync(string id);
    }
}
