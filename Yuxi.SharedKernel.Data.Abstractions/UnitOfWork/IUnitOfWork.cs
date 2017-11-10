namespace Yuxi.SharedKernel.Data.Abstractions.UnitOfWork
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IUnitOfWork : IDisposable
    {
        #region Public Methods

        int SaveChanges(bool ensureAutoHistory = false);

        Task<int> SaveChangesAsync(bool ensureAutoHistory = false);

        int ExecuteSqlCommand(string sql, params object[] parameters);

        IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class;

        #endregion
    }
}