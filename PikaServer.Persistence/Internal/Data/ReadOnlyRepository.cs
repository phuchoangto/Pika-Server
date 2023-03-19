using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PikaServer.Domain.Entities;
using PikaServer.Persistence.Internal.Abstracts;

namespace PikaServer.Persistence.Internal.Data;

public abstract class ReadOnlyRepository<TEntity, TContext> : IReadOnlyRepository<TEntity>
	where TEntity : RootEntityBase
	where TContext : DbContext
{
	protected readonly TContext DbContext;
	protected readonly DbSet<TEntity> DbSet;

	protected ReadOnlyRepository(TContext context)
	{
		DbContext = context;
		DbSet = DbContext.Set<TEntity>();
	}


	public void Dispose()
	{
		DbContext.Dispose();
	}

	public virtual async Task<TEntity?> FindOneAsync(object?[]? keyValues,
		CancellationToken cancellationToken = default)
	{
		var entity = await DbSet.FindAsync(keyValues, cancellationToken)
			.ConfigureAwait(false);

		return entity;
	}

	public virtual Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate,
		CancellationToken cancellationToken = default)
	{
		return FindOneAsync(predicate, default, cancellationToken);
	}

	public virtual Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include,
		CancellationToken cancellationToken = default)
	{
		return FindOneAsync(predicate, include, default, cancellationToken);
	}

	public virtual Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include, bool asTracking,
		CancellationToken cancellationToken = default)
	{
		return FindOneAsync(predicate, include, asTracking, default, cancellationToken);
	}

	public virtual Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include, bool asTracking,
		bool ignoreQueryFilters = true,
		CancellationToken cancellationToken = default)
	{
		return FindOneAsync(predicate, p => p, include, asTracking, ignoreQueryFilters, cancellationToken);
	}

	public virtual async Task<TProject?> FindOneAsync<TProject>(Expression<Func<TEntity, bool>> predicate,
		Expression<Func<TEntity, TProject>> selector,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include,
		bool asTracking,
		bool ignoreQueryFilters = true,
		CancellationToken cancellationToken = default)
	{
		return await BuildIQueryable(DbSet, include, asTracking, ignoreQueryFilters)
			.Where(predicate)
			.Select(selector)
			.FirstOrDefaultAsync(cancellationToken);
	}

	private static IQueryable<TEntity> BuildIQueryable(
		IQueryable<TEntity> queryable,
		Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include,
		bool asTracking,
		bool ignoreFilter
	)
	{
		// tracking
		if (!asTracking)
		{
			queryable = queryable.AsNoTracking();
		}

		// include
		if (include is not null)
		{
			queryable = include(queryable);
		}

		// ignoreFilter
		if (ignoreFilter)
		{
			queryable = queryable.IgnoreQueryFilters();
		}

		return queryable;
	}

}
