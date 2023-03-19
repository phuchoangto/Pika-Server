using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PikaServer.Api.Schemas;
using PikaServer.Domain.Entities;
using PikaServer.Infras.Services.Interfaces;
using PikaServer.Infras.Services.Notification;
using PikaServer.Persistence.Internal.Abstracts;

namespace PikaServer.Api.Controllers;

public class TransactionController : ApiV1ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHdBankBasicFeature _hdBankBasicFeature;
    private readonly IJwtAuthService _jwtAuthService;
    private readonly INotificationService _notificationService;

    public TransactionController(IUnitOfWork unitOfWork, IHdBankBasicFeature hdBankBasicFeature, IJwtAuthService jwtAuthService, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _hdBankBasicFeature = hdBankBasicFeature;
        _jwtAuthService = jwtAuthService;
        _notificationService = notificationService;
    }

    [HttpPost("transfer")]
    [Authorize]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        var account = await _jwtAuthService.GetAccountFromClaimsAsync(User.Claims);
        
        if (account == null)
            return BadRequest("Invalid Token");
        
        // transfer money
        var result =
            await _hdBankBasicFeature.TransferAsync(request.Amount, request.Description, account.AccountNo,
                request.ToAccountNo);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        
        // save changes
        var transaction = new Transaction
        {
            Amount = request.Amount,
            FromAccountNo = account.AccountNo,
            ToAccountNo = request.ToAccountNo,
            Description = request.Description,
        };
        var newTransaction = await _unitOfWork.Transaction.InsertAsync(transaction);
        
        // create notification
        var notification = new Notification
        {
            AccountId = request.ToAccountNo,
            Message = $"You have received {request.Amount} from {account.FullName}",
            Title = $"Received {request.Amount}",
            CreatedAt = DateTime.Now,
            Type = NotificationType.Transaction
        };
        
        var newNotification = await _unitOfWork.Notification.InsertAsync(notification);
        
        await _unitOfWork.CommitAsync();
        return Ok(newTransaction);
    }

    [HttpGet("history")]
    [Authorize]
    public async Task<IActionResult> History()
    {
        var account = await _jwtAuthService.GetAccountFromClaimsAsync(User.Claims);
        
        if (account == null)
            return BadRequest("Invalid Token");
        
        var transactions = await _unitOfWork.Transaction.GetByAccountNoAsync(account.AccountNo);
        
        return Ok(transactions);
    }
}