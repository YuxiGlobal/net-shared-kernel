namespace Yuxi.SharedKernel.Data.EntityFrameworkCore.Repository
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Data.Abstractions.PagedList;
    using Abstractions;
    using PagedList;
    using Patterns.Specification.Contracts;

    internal class QueryableRepository<TEntity> : IQueryableRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext DbContext;
        protected readonly DbSet<TEntity> DbSet;

        public QueryableRepository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            DbSet = DbContext.Set<TEntity>();
        }

        public IPagedList<TEntity> GetPagedList(ISpecification<TEntity> businessSpecification = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (businessSpecification != null)
            {
                query = query.Where(entityToTest => businessSpecification.IsSatisfiedBy(entityToTest));
            }

            return orderBy != null
                ? orderBy(query).ToPagedList(pageIndex, pageSize)
                : query.ToPagedList(pageIndex, pageSize);
        }

        public Task<IPagedList<TEntity>> GetPagedListAsync(ISpecification<TEntity> businessSpecification = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<TEntity> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (businessSpecification != null)
            {
                query = query.Where(entityToTest => businessSpecification.IsSatisfiedBy(entityToTest));
            }

            return orderBy?.Invoke(query).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken) ??
                   query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
        }

        public IPagedList<TResult> GetPagedList<TResult>(Expression<Func<TEntity, TResult>> selector,
            ISpecification<TEntity> businessSpecification = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true)
            where TResult : class
        {
            IQueryable<TEntity> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (businessSpecification != null)
            {
                query = query.Where(entityToTest => businessSpecification.IsSatisfiedBy(entityToTest));
            }

            return orderBy != null
                ? orderBy(query).Select(selector).ToPagedList(pageIndex, pageSize)
                : query.Select(selector).ToPagedList(pageIndex, pageSize);
        }

        public Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            ISpecification<TEntity> businessSpecification = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken))
            where TResult : class
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (businessSpecification != null)
            {
                query = query.Where(entityToTest => businessSpecification.IsSatisfiedBy(entityToTest));
            }

            return orderBy?.Invoke(query).Select(selector)
                       .ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken) ?? query.Select(selector)
                       .ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
        }

        public TEntity GetFirstOrDefault(ISpecification<TEntity> businessSpecification = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (businessSpecification != null)
            {
                query = query.Where(entityToTest => businessSpecification.IsSatisfiedBy(entityToTest));
            }

            return orderBy != null ? orderBy(query).FirstOrDefault() : query.FirstOrDefault();
        }

        public TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
            ISpecification<TEntity> businessSpecification = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (businessSpecification != null)
            {
                query = query.Where(entityToTest => businessSpecification.IsSatisfiedBy(entityToTest));
            }

            return orderBy != null
                ? orderBy(query).Select(selector).FirstOrDefault()
                : query.Select(selector).FirstOrDefault();
        }

        public IQueryable<TEntity> FromSql(string sql, params object[] parameters) => DbSet.FromSql(sql, parameters);

        public TEntity Find(params object[] keyValues) => DbSet.Find(keyValues);

        public Task<TEntity> FindAsync(params object[] keyValues) => DbSet.FindAsync(keyValues);

        public Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken) =>
            DbSet.FindAsync(keyValues, cancellationToken);

        public int Count(Expression<Func<TEntity, bool>> predicate = null) =>
            predicate == null ? DbSet.Count() : DbSet.Count(predicate);
    }
}