using AvitoCoinShop.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AvitoCoinShop.Presentation.Http.Controllers;

[Route("api/buy")]
[ApiController]
public class BuyController : ControllerBase
{
    private readonly IMerchService _merchService;

    public BuyController(IMerchService merchService)
    {
        _merchService = merchService;
    }
    
    [HttpGet("{item}")]
    public async Task<IActionResult> BuyItemAsync(string itemName, CancellationToken cancellationToken)
    {
        var userIdString = GetUserIdFromToken();

        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(itemName))
        {
            return BadRequest(new { errors = "Invalid request: item name required" });
        }

        try
        {
            long userId = long.Parse(userIdString);
            var userMerchItemId = await _merchService.BuyMerchAsync(userId, itemName, cancellationToken);

            return Ok(userMerchItemId);
            
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    private string GetUserIdFromToken()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return userId;
    }
}
