using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Services {
    public interface IPipelineHubNotifier {
        public Task NotifyAllAsync(string methodName, object message);
    }
    public class PipelineHubNotifier : IPipelineHubNotifier {
        private readonly IHubContext<PipelineHub> _hub;
        public PipelineHubNotifier(IHubContext<PipelineHub> hubContext) {
            _hub = hubContext;
        }
        public async Task NotifyAllAsync(string methodName, object message) {
            if (string.IsNullOrWhiteSpace(methodName) || message == null)
                return;

            await _hub.Clients.All.SendAsync(methodName, message);
        }
    }
}
