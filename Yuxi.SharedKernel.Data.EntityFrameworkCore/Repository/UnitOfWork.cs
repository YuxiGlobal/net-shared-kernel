namespace Yuxi.SharedKernel.Data.EntityFrameworkCore.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Abstractions.Repository;
    using Abstractions.UnitOfWork;

    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext> where TContext : DbContext
    {
        private bool _disposed;
        private Dictionary<Type, object> _commandRepositories;
        private Dictionary<Type, object> _queryableRepositories;

        public TContext DbContext { get; }

        public UnitOfWork(TContext context)
        {
            DbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

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

        public IQueryableRepository<TEntity> GetQueryableRepository<TEntity>() where TEntity : class
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

            return (IQueryableRepository<TEntity>) _queryableRepositories[type];
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
                        uow.DbContext.Database.UseTransaction(transaction.GetDbTransaction());
                        count += await uow.SaveChangesAsync(ensureAutoHistory);
                    }

                    count += await SaveChangesAsync(ensureAutoHistory);

                    transaction.Commit();

                    return count;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw ex;
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
    }
}