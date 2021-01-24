using Core.Models;
using Core.Repositories;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services {
    public class PipelineTaskService : IPipelineTaskService {
        private readonly IPipelineTaskRepository _repository;
        private readonly IPipelineRepository _pipelineRepository;
        public PipelineTaskService(IPipelineTaskRepository repository, IPipelineRepository pipelineRepository) {
            _repository = repository;
            _pipelineRepository = pipelineRepository;
        }
        public async Task<IEnumerable<IPipelineTask>> GetAllAsync() {
            return await _repository.GetAllAsync();
        }
        public async Task CreateAsync(IPipelineTask task, string userId) {
            if (task == null)
                return;
            task.CreatedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(userId))
                task.UserId = userId;
            await _repository.CreateAsync(task);
        }

        public async Task<IPipelineTask> GetAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            return await _repository.GetAsync(id);
        }

        public async Task RemoveAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return;
            var pipelines = (await _pipelineRepository.GetAllAsync()).ToList();
            var affectedPipelines = pipelines.Select(x => x).Where(x => x.TaskIds.Contains(id)).ToList();
            foreach (var pipeline in affectedPipelines) {
                pipeline.TaskIds = pipeline.TaskIds.Except(new List<string>() { id }).ToList();
                await _pipelineRepository.UpdateAsync(pipeline);
            }
            await _repository.RemoveAsync(id);
        }

        public async Task UpdateAsync(IPipelineTask task) {
            if (task == null)
                return;
            await _repository.UpdateAsync(task);
        }
    }
}
