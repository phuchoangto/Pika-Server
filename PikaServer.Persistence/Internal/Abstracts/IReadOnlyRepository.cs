using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using PikaServer.Domain.Entities;

namespace PikaServer.Persistence.Internal.Abstracts;

public interface IReadOnlyRepository<TEntity> : IDisposable where TEntity : RootEntityBase
{
    #region Find one

    Task<TEntity?> FindOneAsync(object?[]? keyValues, CancellationToken cancellationToken = default);

    Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include,
        CancellationToken cancellationToken = default);

    Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include,
        bool asTracking,
        CancellationToken cancellationToken = default);

    Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include,
        bool asTracking,
        bool ignoreQueryFilters = true,
        CancellationToken cancellationToken = default);

    Task<TProject?> FindOneAsync<TProject>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TProject>> selector,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include,
        bool asTracking,
        bool ignoreQueryFilters = true,
        CancellationToken cancellationToken = default);

    #endregion
}