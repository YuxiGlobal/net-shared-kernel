namespace Yuxi.SharedKernel.Data.EntityFrameworkCore.UnitOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Data.Abstractions.Repository;
    using Data.Abstractions.UnitOfWork;
    using Repository;
    using Abstractions;

    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext> where TContext : DbContext
    {
        #region Private Members

        private bool _disposed;

        private Dictionary<Type, object> _commandRepositories;

        private Dictionary<Type, object> _queryableRepositories;

        #endregion

        #region Public Read Only Properties

        public TContext DbContext { get; }

        #endregion

        #region Constructors

        public UnitOfWork(TContext context)
        {
            DbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region Public Methods

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_commandRepositories == null)
            {
                _commandRepositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!_commandRepositories.ContainsKey(type))
            {
                _commandRepositories[type] = new Repository<TEntity>(DbContext);
            }

            return (IRepository<TEntity>) _commandRepositories[type];
        }

        public Abstractions.IQueryableRepository<TEntity> GetQueryableRepository<TEntity>() where TEntity : class
        {
            if (_queryableRepositories == null)
            {
                _queryableRepositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!_queryableRepositories.ContainsKey(type))
            {
                _queryableRepositories[type] = new QueryableRepository<TEntity>(DbContext);
            }

            return (Abstractions.IQueryableRepository<TEntity>) _queryableRepositories[type];
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters) =>
            DbContext.Database.ExecuteSqlCommand(sql, parameters);

        public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class =>
            DbContext.Set<TEntity>().FromSql(sql, parameters);

        public int SaveChanges(bool ensureAutoHistory = false)
        {
            if (ensureAutoHistory)
            {
                DbContext.EnsureAutoHistory();
            }

            return DbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false)
        {
            if (ensureAutoHistory)
            {
                DbContext.EnsureAutoHistory();
            }

            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false, params IUnitOfWork[] unitOfWorks)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var count = 0;
                    foreach (var unitOfWork in unitOfWorks)
                    {
                        var uow = unitOfWork as UnitOfWork<DbContext>;

                        if (uow == null)
                        {
                            continue;
                        }

                        uow.DbContext.Database.UseTransaction(transaction.GetDbTransaction());
                        count += await uow.SaveChangesAsync(ensureAutoHistory);
                    }

                    count += await SaveChangesAsync(ensureAutoHistory);

                    transaction.Commit();

                    return count;
                }
                catch (Exception)
                {
                    transaction.Rollback();

                    throw;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _commandRepositories?.Clear();
                    _queryableRepositories?.Clear();

                    DbContext.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}