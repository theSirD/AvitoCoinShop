using AvitoCoinShop.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvitoCoinShop.Presentation.Http.Controllers;

[ApiController]
[Authorize]
public class InfoController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly IMerchService _merchService;
    private readonly ITransactionHistoryService _transactionHistoryService;

    public InfoController(
        IWalletService walletService, 
        IMerchService merchService, 
        ITransactionHistoryService transactionHistoryService)
    {
        _walletService = walletService;
        _merchService = merchService;
        _transactionHistoryService = transactionHistoryService;
    }
    
    [HttpGet("api/info")]
    public async Task<IActionResult> GetInfoAsync(CancellationToken cancellationToken)
    {
        var userIdString = GetUserIdFromToken();

        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized();
        }

        try
        {
            long userId = long.Parse(userIdString);
            var balance = await _walletService.GetBalanceAsync(userId, cancellationToken);
            var purchasedItems = await _merchService.GetMerchItemsBoughtByUser(userId, cancellationToken);
            var transactionHistory = await _transactionHistoryService.GetTransactionHistorySummaryAsync(userId, cancellationToken);

            var info = new 
            {
                coins = balance,
                inventory = purchasedItems,
                coinHistory = transactionHistory
            };

            return Ok(info);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    private string? GetUserIdFromToken()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return userId;
    }
}