using System.Collections.Generic;

namespace CyntegrityDemoNetCore.Models {
    public class TaskListViewModel {
        public IEnumerable<PipelineTask> Tasks { get; set; }
    }
}
