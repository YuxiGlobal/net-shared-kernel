namespace Yuxi.SharedKernel.Data.Abstractions.Repository
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRepository<in TEntity> where TEntity : class
    {
        #region Public Methods

        #region Inserts

        void Insert(TEntity entity);

        void Insert(params TEntity[] entities);

        void Insert(IEnumerable<TEntity> entities);

        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        Task InsertAsync(params TEntity[] entities);

        Task InsertAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region Updates

        void Update(TEntity entity);

        void Update(params TEntity[] entities);

        void Update(IEnumerable<TEntity> entities);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(params TEntity[] entities);

        Task UpdateAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region Deletes

        void Delete(object id);

        void Delete(TEntity entity);

        void Delete(params TEntity[] entities);

        void Delete(IEnumerable<TEntity> entities);

        Task DeleteAsync(object id);

        Task DeleteAsync(TEntity entity);

        Task DeleteAsync(params TEntity[] entities);

        Task DeleteAsync(IEnumerable<TEntity> entities);

        #endregion

        #endregion
    }
}