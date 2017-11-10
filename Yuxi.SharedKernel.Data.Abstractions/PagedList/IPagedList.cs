namespace Yuxi.SharedKernel.Data.Abstractions.PagedList
{
    using System.Collections.Generic;

    public interface IPagedList<T>
    {
        #region Public Read Only Properties

        int IndexFrom { get; }

        int PageIndex { get; }

        int PageSize { get; }

        int TotalCount { get; }

        int TotalPages { get; }

        IList<T> Items { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }

        #endregion
    }
}