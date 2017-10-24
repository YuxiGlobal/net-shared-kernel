namespace Yuxi.SharedKernel.Storage.Implementations
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Data.Contracts.Query;
    using Data.Implementations.Entity;
    using Data.Contracts.Repository;
    using Specification.Base;
    using Specification.Contracts;
    using Specification.Implementations;
    using Contracts;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using TrackableEntities;

    public class AggregateGenericRepositoryReliableStateManager<T, TI> : IRepositoryAsync<T> where T : Entity
        where TI : IStorableItem
    {
        #region Read Only Properties

        private readonly IReliableDictionary<string, TI> _aggreateStorage;
        private readonly ITransaction _transaction;
        private readonly IStorableEntityMapper<T, TI> _mapper;

        #endregion

        #region Constructors

        public AggregateGenericRepositoryReliableStateManager(IReliableDictionary<string, TI> aggreateStorage,
            ITransaction transaction, IStorableEntityMapper<T, TI> mapper)
        {
            _aggreateStorage = aggreateStorage;
            _transaction = transaction;
            _mapper = mapper;
        }

        #endregion

        #region Querys

        public async Task<T> Find(string id)
        {
            var entity = (await _aggreateStorage.TryGetValueAsync(_transaction, id)).Value;
            var coreEntity = _mapper.MapToCore(entity);
            return coreEntity;
        }

        public T Find(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public void UpsertGraph(T entity)
        {
            throw new NotImplementedException();
        }

        public IQueryFluent<T> Query(ExpressionSpecification<T> specification)
        {
            throw new NotImplementedException();
        }

        public IQueryFluent<T> Query()
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Queryable()
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Updates

        public async Task Update(string id, T entity)
        {
            var oldEntityt = (await _aggreateStorage.TryGetValueAsync(_transaction, id)).Value;
            var storableEntity = _mapper.MapToStorable(entity);

            var succeed =
                await _aggreateStorage.TryUpdateAsync(_transaction, entity.Id, storableEntity, oldEntityt);
            if (!succeed) throw new Exception($"Something went wrong when trying to update the entity {id}");
        }

        public void Update(T entity)
        {
            var id = entity.Id;

            Task.Run(() => Update(id, entity));
        }

        #endregion

        #region Adds

        public async Task<T> Add(T entity)
        {
            var storableEntity = _mapper.MapToStorable(entity);
            var succeed = await _aggreateStorage.TryAddAsync(_transaction, entity.Id, storableEntity);

            if (!succeed) throw new Exception("Something went wrong when trying to add the turn");
            return entity;
        }

        void IRepository<T>.Add(T entity)
        {
            Task.Run(() => Add(entity));
        }

        #endregion

        #region Repositories

        public IRepository<T1> GetRepository<T1>() where T1 : class, ITrackable
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Deletes

        public Task Delete(T entity)
        {
            var id = entity.Id;
            return _aggreateStorage.TryRemoveAsync(_transaction, id);
        }

        void IRepository<T>.Delete(T entity)
        {
            Task.Run(() => Delete(entity));
        }

        public void Delete(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Lists

        public Task<IEnumerable<T>> ListAll()
        {
            var allSpecification = new AllSpecification<T>();
            return ListBySpecification(allSpecification);
        }

        public async Task<IEnumerable<T>> ListBySpecification(ISpecification<T> specification)
        {
            var result = new List<T>();

            var allTurns = await _aggreateStorage.CreateEnumerableAsync(_transaction, EnumerationMode.Unordered);

            using (var enumerator = allTurns.GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var current = enumerator.Current;
                    var entity = current.Value;
                    var coreEntity = _mapper.MapToCore(entity);
                    if (specification.IsSatisfiedBy(coreEntity))
                        result.Add(coreEntity);
                }
            }

            return result;
        }

        #endregion
    }
}