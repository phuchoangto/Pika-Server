using Microsoft.EntityFrameworkCore;
using PikaServer.Domain.Entities;
using PikaServer.Persistence.Internal.Abstracts;

namespace PikaServer.Persistence.Internal.Data;

public abstract class RepositoryBase<TEntity, TContext> : WriteRepository<TEntity, TContext>, IRepositoryBase<TEntity>
	where TEntity : RootEntityBase
	where TContext : DbContext
{
	protected RepositoryBase(TContext context) : base(context)
	{
	}

	public IQueryable<TEntity> AsQueryable()
	{
		return DbSet;
	}
}
