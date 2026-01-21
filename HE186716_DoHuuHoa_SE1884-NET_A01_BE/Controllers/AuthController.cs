using Microsoft.AspNetCore.Mvc;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.DTOs;
using HE186716_DoHuuHoa_SE1884_NET_A01_BE.Services;

namespace HE186716_DoHuuHoa_SE1884_NET_A01_BE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(result);
    }
}
