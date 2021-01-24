using Core.Models;
using Core.Repositories;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services {
    public class PipelineService : IPipelineService {
        protected readonly IPipelineRepository _repository;
        private readonly IPipelineTaskService _taskService;
        protected readonly IPipelineRunner _runner;
        public PipelineService(IPipelineRepository repository, IPipelineTaskService taskService, IPipelineRunner runner) {
            _repository = repository;
            _taskService = taskService;
            _runner = runner;
        }
        public async Task<int> CalculateAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return -1;

            var pipeline = await GetAsync(id);
            if (pipeline == null)
                return -1;

            return CalculateAverageTime(pipeline);
        }

        public Task CreateAsync(IPipeline pipeline, string userId) {
            if (pipeline == null || string.IsNullOrWhiteSpace(userId))
                return null;

            pipeline.CreatedAt = DateTime.UtcNow;
            pipeline.UserId = userId;
            return _repository.CreateAsync(pipeline);
        }

        public async Task<IPipeline> GetAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            IPipeline pipeline = await _repository.GetAsync(id);
            if (pipeline != null)
                await GetPipelineTasks(pipeline);
            return pipeline;
        }

        public Task<IEnumerable<IPipeline>> GetAllAsync() {
            return _repository.GetAllAsync();
        }

        public Task RemoveAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return _repository.RemoveAsync(id);
        }

        public virtual async Task<int> RunAsync(string id) {
            int result = -1;
            if (string.IsNullOrWhiteSpace(id))
                return result;

            var pipeline = await _repository.GetAsync(id);
            if (pipeline == null)
                return result;

            result = _runner.Run(pipeline);
            if (result >= 0) {
                pipeline.RunTime = result;
                await _repository.UpdateAsync(pipeline);
            }
            return result;
        }

        public virtual async Task UpdateAsync(IPipeline pipeline) {
            if (pipeline == null)
                return;

            await _repository.UpdateAsync(pipeline);
        }

        protected async Task GetPipelineTasks(IPipeline pipeline) {
            if (pipeline == null)
                return;

            if (pipeline.Tasks == null)
                pipeline.Tasks = new List<IPipelineTask>();

            if (pipeline.TaskIds.ToList().Count > 0) {
                List<IPipelineTask> tasks = new List<IPipelineTask>();
                foreach (var taskId in pipeline.TaskIds)
                    tasks.Add(await _taskService.GetAsync(taskId));
                pipeline.Tasks = tasks;
            }
        }

        protected static int CalculateAverageTime(IPipeline pipeline) {
            if (pipeline == null)
                return -1;

            int result = 0;
            foreach (var task in pipeline.Tasks)
                result += task.AverageTime;
            return result;
        }
    }
}
