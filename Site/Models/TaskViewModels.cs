using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Site.Models {
    public class PipelineTaskViewModel : IPipelineTask {
        public string Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Average time is required")]
        [Range(typeof(int), "0", "2147483647", ErrorMessage = "Average time must be in range 0..214748367")]
        public int AverageTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
    public interface ITaskListViewModel {
        public IEnumerable<IPipelineTask> Tasks { get; set; }
    }
    public class TaskListViewModel : ITaskListViewModel {
        public IEnumerable<IPipelineTask> Tasks { get; set; }
    }
}
