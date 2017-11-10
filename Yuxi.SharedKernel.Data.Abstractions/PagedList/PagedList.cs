namespace Yuxi.SharedKernel.Data.Abstractions.PagedList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PagedList<T> : IPagedList<T>
    {
        #region Public Properties

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public int IndexFrom { get; set; }

        public IList<T> Items { get; set; }
        
        #endregion

        #region Public Read Only Properties

        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

        #endregion

        #region Constructors

        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom)
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException(
                    $"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }

            PageIndex = pageIndex;
            PageSize = pageSize;
            IndexFrom = indexFrom;
            TotalCount = source.Count();
            TotalPages = (int) Math.Ceiling(TotalCount / (double) PageSize);

            Items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
        }

        public PagedList() => Items = new T[0];

        #endregion
    }

    public class PagedList<TSource, TResult> : IPagedList<TResult>
    {
        #region Public Read Only Properties

        public int PageIndex { get; }

        public int PageSize { get; }

        public int TotalCount { get; }

        public int TotalPages { get; }

        public int IndexFrom { get; }

        public IList<TResult> Items { get; }

        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

        #endregion

        #region Constructors

        public PagedList(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter,
            int pageIndex, int pageSize, int indexFrom)
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException(
                    $"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }

            PageIndex = pageIndex;
            PageSize = pageSize;
            IndexFrom = indexFrom;
            TotalCount = source.Count();
            TotalPages = (int) Math.Ceiling(TotalCount / (double) PageSize);

            var items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

            Items = new List<TResult>(converter(items));
        }

        public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            PageIndex = source.PageIndex;
            PageSize = source.PageSize;
            IndexFrom = source.IndexFrom;
            TotalCount = source.TotalCount;
            TotalPages = source.TotalPages;

            Items = new List<TResult>(converter(source.Items));
        }

        #endregion
    }

    public static class PagedList
    {
        #region Public Static Methods

        public static IPagedList<T> Empty<T>() => new PagedList<T>();

        public static IPagedList<TResult> From<TResult, TSource>(IPagedList<TSource> source,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter) =>
            new PagedList<TSource, TResult>(source, converter);

        #endregion
    }
}