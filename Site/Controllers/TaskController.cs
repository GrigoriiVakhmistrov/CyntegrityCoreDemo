using Core.Services;
using Site.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Controllers {
    [Authorize]
    public class TaskController : BaseController {
        private readonly IPipelineTaskService _taskService;
        public TaskController(IPipelineTaskService taskService, IAuthorizationService authorizationService) : base(authorizationService) {
            _taskService = taskService;
        }
        public async Task<IActionResult> IndexAsync() {
            var tasks = await _taskService.GetAllAsync();
            return View(new TaskListViewModel { Tasks = tasks.ToList() });
        }
        [Route("[controller]/{id}")]
        [HttpGet]
        public async Task<IActionResult> ShowAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var task = await _taskService.GetAsync(id);
            if (task == null)
                return new NotFoundResult();
            return View(task);
        }
        [Route("[controller]/Add")]
        [HttpGet]
        public IActionResult Add() {
            return View();
        }
        [Route("[controller]/Add")]
        [HttpPost]
        public async Task<IActionResult> AddAsync(PipelineTaskViewModel task) {
            if (ModelState.IsValid) {
                await _taskService.CreateAsync(task, GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View(task);
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpGet]
        public async Task<IActionResult> EditAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var task = await _taskService.GetAsync(id);
            if (task == null)
                return new NotFoundResult();
            if (!await CheckPermissionAsync(task))
                return new ChallengeResult();

            return View(task);
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpPost]
        public async Task<IActionResult> EditAsync(PipelineTaskViewModel task) {
            if (ModelState.IsValid) {
                //we have no guarantee that task.UserId was not spoofed
                var taskForAuth = await _taskService.GetAsync(task.Id);
                if (task == null)
                    return new NotFoundResult();
                if (!await CheckPermissionAsync(taskForAuth))
                    return new ChallengeResult();

                await _taskService.UpdateAsync(task);
                return RedirectToAction("Show", new { id = task.Id });
            }
            return View(task);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var task = await _taskService.GetAsync(id);
            if (task == null)
                return new NotFoundResult();
            if (!await CheckPermissionAsync(task))
                return new ChallengeResult();
            
            await _taskService.RemoveAsync(id);
            return RedirectToAction("Index");
        }
    }
}
