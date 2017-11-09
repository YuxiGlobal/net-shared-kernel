namespace Yuxi.SharedKernel.Data.EntityFrameworkCore.Abstractions
{
    using Data.Abstractions.Repository;

    public interface IRepositoryFactory
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        IQueryableRepository<TEntity> GetQueryableRepository<TEntity>() where TEntity : class;
    }
}