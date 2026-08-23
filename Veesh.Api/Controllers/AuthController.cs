using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Veesh.Core.DTOs.Auth;
using Veesh.Core.DTOs.User;
using Veesh.Core.Services;

namespace Veesh.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
        {
            return await authService.Login(request);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
        {
            return await authService.Register(request);
        }
    }
}