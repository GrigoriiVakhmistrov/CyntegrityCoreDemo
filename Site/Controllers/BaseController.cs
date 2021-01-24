using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Site.Controllers {
    public class BaseController : Controller {
        private readonly IAuthorizationService _authorizationService;

        public BaseController(IAuthorizationService service) {
            _authorizationService = service;
        }
        public async Task<bool> CheckPermissionAsync(object resource) {
            return (await _authorizationService.AuthorizeAsync(User, resource, "EditPolicy")).Succeeded;
        }
        public string GetCurrentUserId() {
            return User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
