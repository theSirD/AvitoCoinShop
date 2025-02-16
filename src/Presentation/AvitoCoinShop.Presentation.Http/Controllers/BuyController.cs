using AvitoCoinShop.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvitoCoinShop.Presentation.Http.Controllers;

[Route("api/buy")]
[ApiController]
[Authorize]
public class BuyController : ControllerBase
{
    private readonly IMerchService _merchService;

    public BuyController(IMerchService merchService)
    {
        _merchService = merchService;
    }
    
    [HttpGet("{item}")]
    public async Task<IActionResult> BuyItemAsync(string item, CancellationToken cancellationToken)
    {
        var userIdString = GetUserIdFromToken();

        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(item))
        {
            return BadRequest(new { errors = "Invalid request: item name required" });
        }

        try
        {
            long userId = long.Parse(userIdString);
            var merchId = await _merchService.BuyMerchAsync(userId, item, cancellationToken);

            return Ok(merchId);
            
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    private string? GetUserIdFromToken()
    {
        var user = User;
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return userId;
    }
}
