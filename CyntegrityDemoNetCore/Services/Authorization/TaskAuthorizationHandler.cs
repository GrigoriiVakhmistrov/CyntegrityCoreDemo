using CyntegrityDemoNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CyntegrityDemoNetCore.Services {
    public class TaskAuthorizationHandler : AuthorizationHandler<SameAuthorRequirement, PipelineTask> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       SameAuthorRequirement requirement,
                                                       PipelineTask resource) {
            if (context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value == resource.UserId) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class SameAuthorRequirement : IAuthorizationRequirement { }
}
