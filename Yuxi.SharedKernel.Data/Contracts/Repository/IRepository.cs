namespace Yuxi.SharedKernel.Data.Contracts.Repository
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Specification.Contracts;
    using Specification.Base;
    using Query;
    using TrackableEntities;

    public interface IRepository<TAgreggate> where TAgreggate : class, ITrackable
    {
        #region Public Methods

        IRepository<T> GetRepository<T>() where T : class, ITrackable;

        #region Commands

        void Add(TAgreggate entity);

        void Update(TAgreggate entity);

        void Delete(TAgreggate entity);

        void Delete(params object[] keyValues);

        TAgreggate Find(params object[] keyValues);

        void UpsertGraph(TAgreggate entity);

        #endregion

        #region Querys

        IQueryFluent<TAgreggate> Query(ExpressionSpecification<TAgreggate> specification);

        IQueryFluent<TAgreggate> Query();

        IQueryable<TAgreggate> Queryable();

        Task<IEnumerable<TAgreggate>> ListAll();

        Task<IEnumerable<TAgreggate>> ListBySpecification(ISpecification<TAgreggate> specification);

        #endregion

        #endregion
    }
}