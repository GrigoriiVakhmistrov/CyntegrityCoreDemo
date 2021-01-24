using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Site.Models {
    public class PipelineViewModel : IPipeline {
        public string Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public IEnumerable<string> TaskIds { get; set; } = new List<string>();
        public IEnumerable<IPipelineTask> Tasks { get; set; } = new List<IPipelineTask>();
        public DateTime CreatedAt { get; set; }
        public int RunTime { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }

    public interface IPipelineListViewModel {
        public IEnumerable<IPipeline> Pipelines { get; set; }
    }
    public class PipelineListViewModel : IPipelineListViewModel {
        public IEnumerable<IPipeline> Pipelines { get; set; }
    }

    public interface IPipelineEditViewModel {
        public IPipeline Pipeline { get; set; }
        public IEnumerable<IPipelineTask> Tasks { get; set; }
    }
    public class PipelineEditViewModel : IPipelineEditViewModel {
        public IPipeline Pipeline { get; set; }
        public IEnumerable<IPipelineTask> Tasks { get; set; } 
    }
}
