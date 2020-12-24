using System.Collections.Generic;

namespace CyntegrityDemoNetCore.Models {
    public class PipelineListViewModel {
        public IEnumerable<Pipeline> Pipelines { get; set; }
    }
    
    public class PipelineEditViewModel {
        public Pipeline Pipeline { get; set; }
        public IEnumerable<PipelineTask> Tasks { get; set; } 
    }
}
