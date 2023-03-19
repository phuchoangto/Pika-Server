using PikaServer.Persistence.Context;
using PikaServer.Persistence.Internal.Abstracts;
using PikaServer.Persistence.Repositories;

namespace PikaServer.Persistence.Internal.Data;

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _appDbContext;

	public UnitOfWork(AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}

	public void Dispose()
	{
		_appDbContext.Dispose();
	}

	public IAccountRepository Account => new AccountRepository(_appDbContext);
	
	public ITransactionRepository Transaction => new TransactionRepository(_appDbContext);
	public INotificationRepository Notification => new NotificationRepository(_appDbContext);
	public IContactRepository Contact => new ContactRepository(_appDbContext);

	public async Task<int> CommitAsync()
	{
		return await _appDbContext.SaveChangesAsync();
	}
}
