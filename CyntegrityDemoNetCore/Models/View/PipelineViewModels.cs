using System.Collections.Generic;

namespace CyntegrityDemoNetCore.Models {
    public class PipelineListViewModel {
        public List<Pipeline> Pipelines { get; set; }
    }
    
    public class PipelineEditViewModel {
        public Pipeline Pipeline { get; set; }
        public List<PipelineTask> Tasks { get; set; } 
    }
}
