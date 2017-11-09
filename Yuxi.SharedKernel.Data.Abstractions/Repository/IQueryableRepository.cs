namespace Yuxi.SharedKernel.Data.Abstractions.Repository
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IQueryableRepository<TEntity>
    {
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);

        TEntity Find(params object[] keyValues);

        Task<TEntity> FindAsync(params object[] keyValues);

        Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken);

        int Count(Expression<Func<TEntity, bool>> predicate = null);
    }
}