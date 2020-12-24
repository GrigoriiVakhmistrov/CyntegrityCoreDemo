using CyntegrityDemoNetCore.Hubs;
using CyntegrityDemoNetCore.Models;
using Microsoft.AspNetCore.SignalR;

namespace CyntegrityDemoNetCore.Services {
    public class PipelineCalculator {
        private readonly IHubContext<PipelineHub> _hub;
        public PipelineCalculator(IHubContext<PipelineHub> hubContext) {
            _hub = hubContext;
        }
        public int Calculate(Pipeline p, bool isNotify = false) {
            int result = 0;
            foreach (var task in p.Tasks)
                result += task.AverageTime;

            if (isNotify) {
                _hub.Clients.All.SendAsync("pipeline", new PipelineNotification() {
                    Pipeline = p,
                    CalculatedTime = result
                });
            }

            return result;
        }
    }

    public class PipelineNotification {
        public Pipeline Pipeline { get; set; }
        public int CalculatedTime { get; set; }
    }
}
