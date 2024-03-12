using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Common.Abstraction;
using SimpleLibrary.Common.Requests;
using IdentityConstants = SimpleLibrary.Api.Identity.IdentityConstants;

namespace SimpleLibrary.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize( Policy= IdentityConstants.AdminUserPolicyName)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var user = await _userService.Create(request);
        return Ok(user);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAll();
        return Ok(users);
    }
}