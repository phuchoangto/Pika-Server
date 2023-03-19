using PikaServer.Persistence.Repositories;

namespace PikaServer.Persistence.Internal.Abstracts;

public interface IUnitOfWork : IDisposable
{
	IAccountRepository Account { get; }
	ITransactionRepository Transaction { get; }
	INotificationRepository Notification { get; }
	IContactRepository Contact { get; }
	Task<int> CommitAsync();
}
