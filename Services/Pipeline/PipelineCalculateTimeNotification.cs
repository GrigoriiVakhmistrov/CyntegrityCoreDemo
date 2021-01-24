using Core.Models;

namespace Services {
    public class PipelineCalculateTimeNotification {
        public IPipeline Pipeline { get; set; }
        public int CalculatedTime { get; set; }
    }
}
