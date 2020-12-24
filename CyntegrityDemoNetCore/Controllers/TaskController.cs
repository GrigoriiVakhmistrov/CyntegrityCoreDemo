using CyntegrityDemoNetCore.Models;
using CyntegrityDemoNetCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CyntegrityDemoNetCore.Controllers {
    [Authorize]
    public class TaskController : BaseController {
        private readonly PipelineTaskService _db;

        public TaskController(PipelineTaskService context, IAuthorizationService authorizationService) : base(authorizationService) {
            _db = context;
        }
        public async System.Threading.Tasks.Task<IActionResult> IndexAsync() {
            var tasks = await _db.GetTasks();
            return View(new TaskListViewModel { Tasks = tasks.ToList() });
        }
        [Route("[controller]/{id}")]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> ShowAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var task = await _db.Get(id);
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
        public async System.Threading.Tasks.Task<IActionResult> AddAsync(PipelineTask task) {
            if (ModelState.IsValid) {
                task.UserId = GetCurrentUserId();
                await _db.Create(task);
                return RedirectToAction("Index");
            }
            return View(task);
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> EditAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var task = await _db.Get(id);
            if (task == null)
                return new NotFoundResult();
            if (!await CheckPermissionAsync(task))
                return new ChallengeResult();

            return View(task);
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> EditAsync(PipelineTask task) {
            if (ModelState.IsValid) {
                //we have no guarantee that task.UserId was not spoofed
                var taskForAuth = await _db.Get(task.Id.ToString());
                if (task == null)
                    return new NotFoundResult();
                if (!await CheckPermissionAsync(taskForAuth))
                    return new ChallengeResult();

                await _db.Update(task);
                return RedirectToAction("Show", new { id = task.Id.ToString() });
            }
            return View(task);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> DeleteAsync(string id) {
            if (ModelState.IsValid) {
                var task = await _db.Get(id);
                if (task == null)
                    return new NotFoundResult();
                if (!await CheckPermissionAsync(task))
                    return new ChallengeResult();

                await _db.Remove(id);
            }
            return RedirectToAction("Index");
        }
    }
}
