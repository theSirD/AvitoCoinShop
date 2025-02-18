using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvitoCoinShop.Presentation.Http.Controllers;

[ApiController]
[Authorize]
public class SendCoinController : ControllerBase
{
    private readonly IWalletService _walletService;

    public SendCoinController(IWalletService walletService)
    {
        _walletService = walletService;
    }
    
    [HttpPost("api/sendCoin")]
    public async Task<IActionResult> SendCoinsAsync([FromBody] SendCoinsRequest request, CancellationToken cancellationToken)
    {
        string userIdString = GetUserIdFromToken();

        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized();
        }

        if (request.Amount <= 0 || string.IsNullOrEmpty(request.ToUser))
        {
            return BadRequest(new { errors = "Invalid response." });
        }

        try
        {
            long userId = long.Parse(userIdString);
            var transactionId = await _walletService.TransferCoinsAsync(userId, request.ToUser, request.Amount, cancellationToken);
            
            return Ok(transactionId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    private string GetUserIdFromToken()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return userId;
    }
}