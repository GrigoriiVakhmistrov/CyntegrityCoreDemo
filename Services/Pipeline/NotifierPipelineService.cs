using Core.Models;
using Core.Repositories;
using Core.Services;
using System.Threading.Tasks;

namespace Services {
    public class NotifierPipelineService : PipelineService {
        private readonly IPipelineHubNotifier _notifier;
        public NotifierPipelineService(IPipelineRepository repository, IPipelineTaskService taskService, IPipelineRunner runner, IPipelineHubNotifier pipelineNotifier)
            : base(repository, taskService, runner) {
            _notifier = pipelineNotifier;
        }

        public override async Task<int> RunAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return -1;

            var pipeline = await _repository.GetAsync(id);
            if (pipeline == null)
                return -1;

            int runtime = _runner.Run(pipeline);
            if (runtime >= 0) {
                pipeline.RunTime = runtime;
                await _repository.UpdateAsync(pipeline);
                await _notifier.NotifyAllAsync(pipeline.Id, pipeline.Name);
            }
            return runtime;
        }

        public override async Task UpdateAsync(IPipeline pipeline) {
            if (pipeline == null)
                return;

            await GetPipelineTasks(pipeline);
            await _repository.UpdateAsync(pipeline);
            int averageTime = CalculateAverageTime(pipeline);
            await _notifier.NotifyAllAsync("pipeline", new PipelineCalculateTimeNotification() {
                Pipeline = pipeline,
                CalculatedTime = averageTime
            });
        }
    }
}
