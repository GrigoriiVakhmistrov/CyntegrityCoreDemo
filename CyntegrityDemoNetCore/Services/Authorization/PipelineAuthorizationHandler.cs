using CyntegrityDemoNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CyntegrityDemoNetCore.Services {
    public class PipelineAuthorizationHandler : AuthorizationHandler<SameAuthorRequirement, Pipeline> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       SameAuthorRequirement requirement,
                                                       Pipeline resource) {
            if (context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value == resource.UserId) {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
