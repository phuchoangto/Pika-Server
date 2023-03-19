using PikaServer.Domain.Entities;

namespace PikaServer.Persistence.Internal.Abstracts;

public interface IRepositoryBase<TEntity> : IReadOnlyRepository<TEntity>, IWriteRepositoryBase<TEntity>
	where TEntity : RootEntityBase
{
	IQueryable<TEntity> AsQueryable();
}
