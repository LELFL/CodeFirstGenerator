using CodeFirstGenerator.Entities;
using CodeFirstGenerator.Migrations;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace ELF.Shared;

public static class QueryableExpan
{
    public static IQueryable<TEntity> AmisOrderBy<TEntity>(this IQueryable<TEntity> query, object input)
    {
        //Try to sort query if available
        if (input is IAmisSortedRequest sortInput)
        {
            if (!string.IsNullOrWhiteSpace(sortInput.Sorting))
            {
                return query.OrderBy(sortInput.Sorting!);
            }
        }

        //No sorting
        return query;
    }
    public static IQueryable<TEntity> AmisPaging<TEntity>(this IQueryable<TEntity> query, object input)
    {
        //Try to use paging if available
        if (input is IAmisPagedRequest pagedInput)
        {
            if (pagedInput.Skip > 0)
                query = (IQueryable<TEntity>)query.Skip(pagedInput.Skip);
            if (pagedInput.Take > 0)
                query = (IQueryable<TEntity>)query.Take(pagedInput.Take);
        }

        //No paging
        return query;
    }

    public static IQueryable<TEntity> WhereIf<TEntity>(this IQueryable<TEntity> query, bool condition, Expression<Func<TEntity, bool>> predicate)
    {
        if (condition)
        {
            return query.Where(predicate);
        }

        return query;
    }

    /// <summary>
    /// WhereNotNull
    /// </summary>
    public static IQueryable<TEntity> WhereNotNull<TEntity>(this IQueryable<TEntity> query, [NotNull] object? condition, Expression<Func<TEntity, bool>> predicate)
    {
        if (condition is not null)
        {
            return query.Where(predicate);
        }

#pragma warning disable CS8777 // 退出时，参数必须具有非 null 值。
        return query;
#pragma warning restore CS8777 // 退出时，参数必须具有非 null 值。
    }
}
