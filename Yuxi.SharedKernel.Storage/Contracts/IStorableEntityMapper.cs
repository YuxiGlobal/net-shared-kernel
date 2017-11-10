namespace Yuxi.SharedKernel.Storage.Contracts
{
    public interface IStorableEntityMapper<T, TI> where TI : IStorableItem
    {
        #region Internal Properties

        TI MapToStorable(T entity);

        T MapToCore(TI entity);

        #endregion
    }
}