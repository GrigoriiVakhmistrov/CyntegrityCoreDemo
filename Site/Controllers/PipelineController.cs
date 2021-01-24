using Core.Services;
using Site.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Controllers {
    [Authorize]
    public class PipelineController : BaseController {
        private readonly IPipelineService _pipelineService;
        private readonly IPipelineTaskService _taskService;
        public PipelineController(IPipelineService pipelineService, IPipelineTaskService taskService, IAuthorizationService authorizationService)
            : base(authorizationService) {
            _pipelineService = pipelineService;
            _taskService = taskService;
        }
        public async Task<IActionResult> IndexAsync() {
            var pipelines = await _pipelineService.GetAllAsync();
            return View(new PipelineListViewModel { Pipelines = pipelines.ToList() });
        }
        [Route("[controller]/{id}")]
        [HttpGet]
        public async Task<IActionResult> ShowAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var pipeline = await _pipelineService.GetAsync(id);
            if (pipeline == null)
                return new NotFoundResult();
            return View(pipeline);
        }
        [Route("[controller]/Add")]
        [HttpGet]
        public async Task<IActionResult> AddAsync() {
            var tasks = await _taskService.GetAllAsync();
            return View(new TaskListViewModel { Tasks = tasks.ToList() });
        }
        [Route("[controller]/Add")]
        [HttpPost]
        public async Task<IActionResult> AddAsync(PipelineViewModel pipeline) {
            if (ModelState.IsValid) {
                await _pipelineService.CreateAsync(pipeline, GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View(pipeline);
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpGet]
        public async Task<IActionResult> EditAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var pipeline = await _pipelineService.GetAsync(id);
            if (pipeline == null)
                return new NotFoundResult();
            if (!await CheckPermissionAsync(pipeline))
                return new ChallengeResult();

            var tasks = await _taskService.GetAllAsync();
            return View(new PipelineEditViewModel() { Pipeline = pipeline, Tasks = tasks.ToList() });
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpPost]
        public async Task<IActionResult> EditAsync(PipelineViewModel pipeline) {
            if (ModelState.IsValid) {
                //we have no guarantee that pipeline.UserId was not spoofed
                var pipelineForAuth = await _pipelineService.GetAsync(pipeline.Id);
                if (pipelineForAuth == null)
                    return new NotFoundResult();
                if (!await CheckPermissionAsync(pipelineForAuth))
                    return new ChallengeResult();

                await _pipelineService.UpdateAsync(pipeline);
                return RedirectToAction("Show", new { id = pipeline.Id });
            }
            return View(pipeline);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var pipeline = await _pipelineService.GetAsync(id);
            if (pipeline == null)
                return new NotFoundResult();
            if (!await CheckPermissionAsync(pipeline))
                return new ChallengeResult();
            
            await _pipelineService.RemoveAsync(id);
            return RedirectToAction("Index");
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpGet]
        public async Task<IActionResult> CalculateAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            int result = await _pipelineService.CalculateAsync(id);
            if (result < 0)
                return new NotFoundResult();
            return new JsonResult(result);
        }
        [Route("[controller]/{id?}/[action]")]
        [HttpGet]
        public async Task<IActionResult> RunAsync(string id) {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            int result = await _pipelineService.RunAsync(id);
            if (result < 0)
                return new NotFoundResult();
            return new JsonResult(result);
        }
    }
}
