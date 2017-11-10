namespace Yuxi.SharedKernel.Data.EntityFrameworkCore.Abstractions
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Data.Abstractions.Repository;
    using Data.Abstractions.UnitOfWork;

    public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : DbContext
    {
        #region Read Only Properties

        TContext DbContext { get; }

        #endregion

        #region Public Methods

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        IQueryableRepository<TEntity> GetQueryableRepository<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync(bool ensureAutoHistory = false, params IUnitOfWork[] unitOfWorks);

        #endregion
    }
}