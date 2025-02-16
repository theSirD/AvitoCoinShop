using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AvitoCoinShop.Presentation.Http.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AuthAsync([FromBody] AuthRequest authRequest, CancellationToken cancellationToken)
    {
        if (authRequest is null || string.IsNullOrWhiteSpace(authRequest.Username) || string.IsNullOrWhiteSpace(authRequest.Password))
        {
            return BadRequest("Username and Password are required");
        }

        try
        {
            string token = await _authService.AuthenticateAsync(authRequest.Username, authRequest.Password, cancellationToken);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return Unauthorized();
        }
    }
}