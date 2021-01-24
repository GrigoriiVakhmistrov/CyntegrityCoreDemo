using System;
using System.Collections.Generic;

namespace Core.Models {
    public interface IPipeline {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> TaskIds { get; set; }
        public IEnumerable<IPipelineTask> Tasks { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RunTime { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
