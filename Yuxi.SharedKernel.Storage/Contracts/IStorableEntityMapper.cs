namespace Yuxi.SharedKernel.Storage.Contracts
{
    using TrackableEntities;

    public interface IStorableEntityMapper<T, TI> where T : ITrackable
        where TI : IStorableItem
    {
        #region Internal Properties

        TI MapToStorable(T entity);

        T MapToCore(TI entity);

        #endregion
    }
}