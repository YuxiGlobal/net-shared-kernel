namespace Yuxi.SharedKernel.Storage.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Specification.Contracts;
    using Specification.Implementations;
    using Contracts;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Data.Abstractions.Entities;
    using Data.Abstractions.Repository;

    public class AggregateGenericRepositoryReliableStateManager<T, TI> : IRepository<T> where T : Entity
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

        #region Adds

        public async Task InsertAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var storableEntity = _mapper.MapToStorable(entity);
            var succeed = await _aggreateStorage.TryAddAsync(_transaction, entity.Id, storableEntity);

            if (!succeed)
            {
                throw new Exception($"Something went wrong when trying to update the entity {entity.Id}");
            }
        }

        public async Task InsertAsync(params T[] entities)
        {
            foreach (var entity in entities)
            {
                await InsertAsync(entity);
            }
        }

        public async Task InsertAsync(IEnumerable<T> entities,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var entity in entities)
            {
                await InsertAsync(entity, cancellationToken);
            }
        }

        public void Insert(T entity)
        {
            throw new NotImplementedException();
        }

        public void Insert(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Insert(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Updates

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oldEntityt = (await _aggreateStorage.TryGetValueAsync(_transaction, entity.Id)).Value;
            var storableEntity = _mapper.MapToStorable(entity);

            var succeed =
                await _aggreateStorage.TryUpdateAsync(_transaction, entity.Id, storableEntity, oldEntityt);
            if (!succeed) throw new Exception($"Something went wrong when trying to update the entity {entity.Id}");
        }

        public async Task UpdateAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            foreach (var entity in entities)
            {
                await UpdateAsync(entity, cancellationToken);
            }
        }

        public async Task UpdateAsync(IEnumerable<T> entities)
        {
            await UpdateAsync(entities, new CancellationToken());
        }

        public async Task UpdateAsync(params T[] entities)
        {
            foreach (var entity in entities)
            {
                await UpdateAsync(entity);
            }
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void Update(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<T> entities)
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

        public async Task DeleteAsync(T entity)
        {
            await Delete(entity);
        }

        public async Task DeleteAsync(params T[] entities)
        {
            foreach (var entity in entities)
            {
                await Delete(entity);
            }
        }

        public async Task DeleteAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                await Delete(entity);
            }
        }

        public Task DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public void Delete(object id)
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Querys

        public async Task<T> Find(string id)
        {
            var entity = (await _aggreateStorage.TryGetValueAsync(_transaction, id)).Value;
            var coreEntity = _mapper.MapToCore(entity);
            return coreEntity;
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