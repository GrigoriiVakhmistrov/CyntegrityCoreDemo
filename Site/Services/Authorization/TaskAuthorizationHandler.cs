using Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Site.Services {
    public class TaskAuthorizationHandler : AuthorizationHandler<SameAuthorRequirement, IPipelineTask> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       SameAuthorRequirement requirement,
                                                       IPipelineTask resource) {
            if (resource == null)
                return Task.CompletedTask;

            if (context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value == resource.UserId) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
