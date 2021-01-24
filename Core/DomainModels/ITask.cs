using System;

namespace Core.Models {
    public interface IPipelineTask {
        public string Id { get; set; }
        public string Name { get; set; }
        public int AverageTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
