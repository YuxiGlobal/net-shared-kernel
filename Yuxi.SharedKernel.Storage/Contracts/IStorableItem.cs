namespace Yuxi.SharedKernel.Storage.Contracts
{
    using System;

    public interface IStorableItem
    {
        #region Internal Properties

        Guid Id { get; }

        #endregion
    }
}