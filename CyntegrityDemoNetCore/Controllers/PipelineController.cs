using CyntegrityDemoNetCore.Models;
using CyntegrityDemoNetCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CyntegrityDemoNetCore.Controllers {
    [Authorize]
    public class PipelineController : BaseController {
        private readonly PipelineService _db;
        private readonly PipelineTaskService _taskDb;
        private readonly PipelineCalculator _calculator;
        private readonly PipelineRunner _runner;
        public PipelineController(PipelineService context, PipelineTaskService taskContext, IAuthorizationService authorizationService,
            PipelineCalculator calculator, PipelineRunner runner)
            : base(authorizationService) {
            _db = context;
            _taskDb = taskContext;
            _calculator = calculator;
            _runner = runner;
        }
        public async System.Threading.Tasks.Task<IActionResult> IndexAsync() {
            var pipelines = await _db.GetPipelines();
            return View(new PipelineListViewModel { Pipelines = pipelines.ToList() });
        }
        [Route("[controller]/{id}")]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> ShowAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var pipeline = await _db.Get(id);
            if (pipeline == null)
                return new NotFoundResult();
            foreach (var taskId in pipeline.TaskIds)
                pipeline.Tasks.Add(await _taskDb.Get(taskId.ToString()));
            return View(pipeline);
        }
        [Route("[controller]/Add")]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> AddAsync() {
            var tasks = await _taskDb.GetTasks();
            return View(new TaskListViewModel { Tasks = tasks.ToList() });
        }
        [Route("[controller]/Add")]
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> AddAsync(Pipeline pipeline) {
            if (ModelState.IsValid) {
                pipeline.UserId = GetCurrentUserId();
                await _db.Create(pipeline);
                return RedirectToAction("Index");
            }
            return View(pipeline);
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> EditAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var pipeline = await _db.Get(id);
            if (pipeline == null)
                return new NotFoundResult();
            if (!await CheckPermissionAsync(pipeline))
                return new ChallengeResult();

            var tasks = await _taskDb.GetTasks();
            return View(new PipelineEditViewModel() { Pipeline = pipeline, Tasks = tasks.ToList() });
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> EditAsync(Pipeline pipeline) {
            if (ModelState.IsValid) {
                //we have no guarantee that pipeline.UserId was not spoofed
                var pipelineForAuth = await _db.Get(pipeline.Id.ToString());
                if (pipelineForAuth == null)
                    return new NotFoundResult();
                if (!await CheckPermissionAsync(pipelineForAuth))
                    return new ChallengeResult();

                foreach (var taskId in pipeline.TaskIds)
                    pipeline.Tasks.Add(await _taskDb.Get(taskId.ToString()));

                await _db.Update(pipeline);
                _calculator.Calculate(pipeline, true);
                return RedirectToAction("Show", new { id = pipeline.Id.ToString() });
            }
            return View(pipeline);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> DeleteAsync(string id) {
            if (ModelState.IsValid) {
                var pipeline = await _db.Get(id);
                if (pipeline == null)
                    return new NotFoundResult();
                if (!await CheckPermissionAsync(pipeline))
                    return new ChallengeResult();

                await _db.Remove(id);
            }
            return RedirectToAction("Index");
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> CalculateAsync(string id) {
            var pipeline = await _db.Get(id);
            if (pipeline == null)
                return new NotFoundResult();

            foreach (var taskId in pipeline.TaskIds)
                pipeline.Tasks.Add(await _taskDb.Get(taskId.ToString()));

            return new JsonResult(_calculator.Calculate(pipeline));
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> RunAsync(string id) {
            var pipeline = await _db.Get(id);
            if (pipeline == null)
                return new NotFoundResult();

            pipeline.RunTime = await _runner.RunAsync(pipeline);
            await _db.Update(pipeline);
            return new JsonResult(pipeline.RunTime);
        }
    }
}
