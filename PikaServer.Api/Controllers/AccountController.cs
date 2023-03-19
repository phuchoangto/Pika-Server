using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PikaServer.Api.Schemas;
using PikaServer.Domain.Entities;
using PikaServer.Infras.Services.Interfaces;
using PikaServer.Persistence.Internal.Abstracts;
using PikaServer.Persistence.Repositories;

namespace PikaServer.Api.Controllers;

public class AccountController : ApiV1ControllerBase
{

	private readonly IHdBankBasicFeature _hdBankBasicFeature;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IJwtAuthService _jwtAuthService;

	public AccountController(IHdBankBasicFeature hdBankBasicFeature, IUnitOfWork unitOfWork,
		IJwtAuthService jwtAuthService)
	{
		_hdBankBasicFeature = hdBankBasicFeature;
		_unitOfWork = unitOfWork;
		_jwtAuthService = jwtAuthService;
	}

	[HttpGet("balance")]
	[Authorize]
	public async Task<IActionResult> GetBalance(CancellationToken cancellationToken = default)
	{
		// parse access token to get account number
		var accountNo = User.Claims.FirstOrDefault(x => x.Type == "AccountNo")?.Value;
		if (string.IsNullOrEmpty(accountNo))
		{
			return BadRequest("Invalid access token");
		}

		// get balance
		var balance = await _hdBankBasicFeature.GetBalanceAsync(accountNo, cancellationToken);
		
		// remove last 3 character
		// 158029.00
		// 158029
		var balanceStr = balance.ToString();
		balanceStr = balanceStr.Substring(0, balanceStr.Length - 2);

		return Ok(balanceStr);
	}

	[HttpGet("name/{accountNo}")]
	[Authorize]
	public async Task<IActionResult> GetName(string accountNo, CancellationToken cancellationToken = default)
	{
		// get name
		var account = await _unitOfWork.Account.FindOneAsync(x => x.AccountNo == accountNo);

		if (account == null)
		{
			return BadRequest();
		}

		return Ok(account.FullName);
	}

	[HttpGet("contacts")]
	[Authorize]
	public async Task<IActionResult> GetContacts(CancellationToken cancellationToken = default)
	{
		var account = await _jwtAuthService.GetAccountFromClaimsAsync(User.Claims);

		if (account == null)
			return BadRequest("Invalid Token");

		var contacts = await _unitOfWork.Contact.GetContactsByAccountIdAsync(account.Id);

		List<object> result = new();

		foreach (var contact in contacts)
		{
			var contactAccount = await _unitOfWork.Account.FindOneAsync(x => x.AccountNo == contact.ContactAccountNo);
			if (contactAccount != null)
			{
				result.Add(new
				{
					AccountNo = contact.ContactAccountNo,
					FullName = contactAccount.FullName,
				});
			}
		}

		return Ok(result);
	}

	[HttpPost("add-contact/{accountNo}")]
	[Authorize]
	public async Task<IActionResult> AddContact(string accountNo, CancellationToken cancellationToken = default)
	{
		var account = await _jwtAuthService.GetAccountFromClaimsAsync(User.Claims);

		if (account == null)
			return BadRequest("Invalid Token");

		var contactAcc = await _unitOfWork.Account.FindOneAsync(x => x.AccountNo == accountNo);
		if (contactAcc == null)
		{
			return BadRequest("Invalid account number");
		}

		var contact = new Contact
		{
			AccountId = account.Id,
			ContactAccountNo = contactAcc.AccountNo,
		};

		var newContact = await _unitOfWork.Contact.InsertAsync(contact);

		return Ok(newContact);
	}

	[HttpGet("notifications")]
	[Authorize]
	public async Task<IActionResult> GetNotifications(CancellationToken cancellationToken = default)
	{
		var account = await _jwtAuthService.GetAccountFromClaimsAsync(User.Claims);

		if (account == null)
			return BadRequest("Invalid Token");

		var notifications = await _unitOfWork.Notification.GetNotificationsByUserIdAsync(account.AccountNo);

		return Ok(notifications);
	}
	

	[HttpGet("passbook-balance")]
	[Authorize]
	public async Task<IActionResult> GetPassbookBalance(CancellationToken cancellationToken = default)
	{
		var account = await _jwtAuthService.GetAccountFromClaimsAsync(User.Claims);

		if (account == null)
			return BadRequest("Invalid Token");

		return Ok(account.PassbookBalance);
	}
	
	// admin: 045704070000581

	[HttpPost("deposit/{amount}")]
	[Authorize]
	public async Task<IActionResult> Deposit(string amount, CancellationToken cancellationToken = default)
	{
		var account = await _jwtAuthService.GetAccountFromClaimsAsync(User.Claims);

		if (account == null)
			return BadRequest("Invalid Token");
		
		// create transfer from user to admin
		var transfer = await _hdBankBasicFeature.TransferAsync(amount, "Deposit Pika Saving", account.AccountNo, "045704070000581");
		
		// save changes
		var transaction = new Transaction
		{
			Amount = amount,
			FromAccountNo = account.AccountNo,
			ToAccountNo = "045704070000581",
			Description = "Deposit Pika Saving",
		};
		var newTransaction = await _unitOfWork.Transaction.InsertAsync(transaction);

		account.PassbookBalance += double.Parse(amount);
		
		var result = await _unitOfWork.Account.UpdateAsync(account);

		await _unitOfWork.CommitAsync();
		
		return Ok(result);
	}
	
	[HttpPost("withdraw/{amount}")]
	[Authorize]
	public async Task<IActionResult> Withdraw(string amount, CancellationToken cancellationToken = default)
	{
		var account = await _jwtAuthService.GetAccountFromClaimsAsync(User.Claims);

		if (account == null)
			return BadRequest("Invalid Token");
		
		// create transfer from user to admin
		var transfer = await _hdBankBasicFeature.TransferAsync(amount, "Withdraw Pika Saving", "045704070000581", account.AccountNo);
		
		// save changes
		var transaction = new Transaction
		{
			Amount = amount,
			FromAccountNo = "045704070000581",
			ToAccountNo = account.AccountNo,
			Description = "Withdraw Pika Saving",
		};
		var newTransaction = await _unitOfWork.Transaction.InsertAsync(transaction);

		account.PassbookBalance -= double.Parse(amount);
		
		var result = await _unitOfWork.Account.UpdateAsync(account);
		
		var notification = new Notification
		{
			AccountId = account.AccountNo,
			Title = "Withdraw Pika Saving",
			Message = $"You have withdraw {amount} VND from your Pika Saving account",
		};
		
		var newNotification = await _unitOfWork.Notification.InsertAsync(notification);
		

		await _unitOfWork.CommitAsync();
		
		return Ok(result);
	}
	
	[HttpPost("check-contacts")]
	[Authorize]
	public async Task<IActionResult> CheckContacts([FromBody] CheckContactRequest request, CancellationToken cancellationToken = default)
	{
		var account = await _jwtAuthService.GetAccountFromClaimsAsync(User.Claims);

		if (account == null)
			return BadRequest("Invalid Token");
	
		List<object> result = new();
		
		foreach (var phone in request.phones)
		{
			var contactAcc = await _unitOfWork.Account.FindOneAsync(x => x.Phone == phone);
			if (contactAcc != null)
			{
				result.Add(new
				{
					AccountNo = contactAcc.AccountNo,
					FullName = contactAcc.FullName,
				});
			}
		}
		
		return Ok(result);
	}
}