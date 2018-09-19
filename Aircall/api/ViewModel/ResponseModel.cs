using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace api.ViewModel
{
    public class ResponseModel
    {
        public int StatusCode { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public Nullable<DateTime> LastCallDateTime { get; set; }
    }
    public class PagedResults<T>
    {
        /// <summary>
        /// The page number this page represents.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The size of this page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of pages available.
        /// </summary>
        public int TotalNumberOfPages { get; set; }

        /// <summary>
        /// The total number of records available.
        /// </summary>
        public int TotalNumberOfRecords { get; set; }

        /// <summary>
        /// The URL to the next page - if null, there are no more pages.
        /// </summary>
        public string NextPageUrl { get; set; }

        /// <summary>
        /// The records this page represents.
        /// </summary>
        public IEnumerable<T> Results { get; set; }
    }
    public class ResponseListModel
    {
        public int StatusCode { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long TotalNumberOfPages { get; set; }
        public long TotalNumberOfRecords { get; set; }
        public object Data { get; set; }
        public Nullable<DateTime> LastCallDateTime { get; set; }
        public bool HasPaymentFailedUnit { get ; set; }
        public bool HasPaymentProcessingUnits { get; set; }
    }

    public static class Extensions
    {
        /// <summary>
        /// Order the IQueryable by the given property or field.
        /// </summary>

        /// <typeparam name="T">The type of the IQueryable being ordered.</typeparam>
        /// <param name="queryable">The IQueryable being ordered.</param>
        /// <param name="propertyOrFieldName">
        /// The name of the property or field to order by.</param>
        /// <param name="ascending">Indicates whether or not 
        /// the order should be ascending (true) or descending (false.)</param>
        /// <returns>Returns an IQueryable ordered by the specified field.</returns>
        public static IQueryable<T> OrderByPropertyOrField<T>
        (this IQueryable<T> queryable, string propertyOrFieldName, bool ascending = true)
        {
            var elementType = typeof(T);
            var orderByMethodName = ascending ? "OrderBy" : "OrderByDescending";

            var parameterExpression = Expression.Parameter(elementType);
            var propertyOrFieldExpression =
                Expression.PropertyOrField(parameterExpression, propertyOrFieldName);
            var selector = Expression.Lambda(propertyOrFieldExpression, parameterExpression);

            var orderByExpression = Expression.Call(typeof(Queryable), orderByMethodName,
                new[] { elementType, propertyOrFieldExpression.Type }, queryable.Expression, selector);

            return queryable.Provider.CreateQuery<T>(orderByExpression);
        }

        public static IQueryable<T> CreatePagedResults<T, TResult>(IQueryable<T> query, int pageNum, int pageSize, out int rowsCount, out int page, out int totalPageCount)
        {
            if (pageSize <= 0) pageSize = 20;
            if (pageNum <= 0) pageNum = 1;
            //Total result count
            rowsCount = query.Count();
            var mod = rowsCount % pageSize;
            totalPageCount = (rowsCount / pageSize) + (mod == 0 ? 0 : 1);
            //If page number should be > 0 else set to first page
            if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            //Calculate nunber of rows to skip on pagesize
            int excludedRows = (pageNum - 1) * pageSize;

            //query = isAscendingOrder ? query.OrderBy(orderByProperty) : query.OrderByDescending(orderByProperty);

            //Skip the required rows for the current page and take the next records of pagesize count
            page = pageNum + 1;
            return query.Skip(excludedRows).Take(pageSize);
        }
    }
}