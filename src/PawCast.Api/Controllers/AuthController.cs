using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PawCast.Api.Auth;
using PawCast.Application.DTOs;

namespace PawCast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DemoUserOptions _demoUserOptions;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(
        IOptions<DemoUserOptions> demoUserOptions,
        JwtTokenService jwtTokenService)
    {
        _demoUserOptions = demoUserOptions.Value;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _demoUserOptions.Users.FirstOrDefault(x =>
            string.Equals(x.Username, request.Username, StringComparison.OrdinalIgnoreCase) &&
            x.Password == request.Password);

        if (user is null)
        {
            return Unauthorized(new
            {
                code = "invalid_credentials",
                message = "Invalid username or password."
            });
        }

        var response = _jwtTokenService.CreateToken(user.Username, user.Role);
        return Ok(response);
    }
}