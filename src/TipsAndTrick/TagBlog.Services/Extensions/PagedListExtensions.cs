using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Collections;
using static TatBlog.Core.Contracts.IPagedList;
using System.Linq.Dynamic.Core;
using TatBlog.Core.Contracts;

namespace TatBlog.Services.Extensions
{
    public static class PagedListExtensions
    {
        public static string GetOrderExpression(
            this IPagingParams pagingParams,
            string defaultColumn = "Id")
        {
            var column = string.IsNullOrWhiteSpace(pagingParams.SortColumn)
                ?defaultColumn
                :pagingParams.SortColumn;
            var order = "ACS".Equals(
                pagingParams.SortOrder, StringComparison.OrdinalIgnoreCase)
                ? pagingParams.SortOrder : "DESC";
            return $"{column} {order}";
        }
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(
            this IQueryable<T> source,
            IPagingParams pagingParams,
            CancellationToken cancellationToken= default
            ) {
            var totalCount = await source.CountAsync(cancellationToken);
            var items = await source
                .OrderBy(pagingParams.GetOrderExpression())
                .Skip((pagingParams.PageNumber-1)*pagingParams.PageSize)
                .Take(pagingParams.PageSize)
                .ToListAsync(cancellationToken);
            return new PagedList<T>(
                items, pagingParams.PageSize, pagingParams.PageNumber, totalCount);
                
                
        }
    }

}
