using System.Linq.Expressions;
using PikaServer.Domain.Entities;

namespace PikaServer.Persistence.Internal.Abstracts;

public interface IWriteRepositoryBase<TEntity> : IDisposable where TEntity : RootEntityBase
{
	#region Delete

	/// <summary>
	/// Delete entity async
	/// </summary>
	/// <param name="entity"></param>
	/// <param name="sendEvent"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<bool> DeleteAsync(TEntity entity,
		CancellationToken cancellationToken = default);

	#endregion


	#region Insert

	/// <summary>
	/// Insert one entity async
	/// </summary>
	/// <param name="entity"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<TEntity?> InsertAsync(TEntity entity,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Insert multiple entity
	/// </summary>
	/// <param name="entities"></param>
	/// <returns></returns>
	Task<bool> InsertBatchAsync(IList<TEntity> entities);

	#endregion

	#region Update

	/// <summary>
	/// Update an entity
	/// </summary>
	/// <param name="entity"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<bool> UpdateAsync(TEntity entity,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Update only one field of record
	/// </summary>
	/// <param name="entity"></param>
	/// <param name="update"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<bool> UpdateOneFieldAsync(
		TEntity entity,
		Expression<Func<TEntity, object>> update,
		CancellationToken cancellationToken = default);

	#endregion
}
