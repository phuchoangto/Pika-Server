using PikaServer.Domain.Entities;
using PikaServer.Persistence.Context;
using PikaServer.Persistence.Internal.Abstracts;
using PikaServer.Persistence.Internal.Data;

namespace PikaServer.Persistence.Repositories;

public interface IAccountRepository : IRepositoryBase<Account>
{
}

public class AccountRepository : RepositoryBase<Account, AppDbContext>, IAccountRepository
{
	public AccountRepository(AppDbContext context) : base(context)
	{
	}
}
