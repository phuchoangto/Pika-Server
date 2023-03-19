using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PikaServer.Domain.Entities;
using PikaServer.Persistence.Internal.Abstracts;

namespace PikaServer.Persistence.Internal.Data;

public abstract class WriteRepository<TEntity, TContext> : ReadOnlyRepository<TEntity, TContext>,
	IWriteRepositoryBase<TEntity>
	where TEntity : RootEntityBase
	where TContext : DbContext
{
	protected WriteRepository(TContext context) : base(context)
	{
	}

	public new void Dispose()
	{
		DbContext.Dispose();
	}

	public virtual async Task<bool> UpdateOneFieldAsync(TEntity entity, Expression<Func<TEntity, object>> update,
		CancellationToken cancellationToken = default)
	{
		DbSet.Attach(entity).Property(update).IsModified = true;
		var save = await DbContext.SaveChangesAsync(cancellationToken) > 0;

		return save;
	}

	public virtual async Task<bool> DeleteAsync(TEntity entity,
		CancellationToken cancellationToken = default)
	{
		DbSet.Attach(entity).State = EntityState.Deleted;
		var saves = await DbContext.SaveChangesAsync(cancellationToken) > 0;

		return saves;
	}

	public virtual async Task<TEntity?> InsertAsync(TEntity entity,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(entity);

		await DbSet.AddAsync(entity, cancellationToken);
		var saves = await DbContext.SaveChangesAsync(cancellationToken);

		return saves <= 0 ? null : entity;
	}

	public virtual async Task<bool> InsertBatchAsync(IList<TEntity> entities)
	{
		ArgumentNullException.ThrowIfNull(entities);

		await DbSet.AddRangeAsync(entities);
		var saves = await DbContext.SaveChangesAsync();

		return saves > 0;
	}

	public virtual async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		DbSet.Attach(entity).State = EntityState.Modified;
		var save = await DbContext.SaveChangesAsync(cancellationToken) > 0;

		return save;
	}
}
