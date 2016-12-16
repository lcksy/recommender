using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace CQSS.Common.Database.Dapper
{
    public static partial class DapperExtension
    {
        public static IEnumerable<TParent> QueryParentChild<TParent, TChild, TParentKey>(
            this IDbConnection connection,
            string sql,
            Func<TParent, TParentKey> parentKeySelector,
            Func<TParent, IList<TChild>> childSelector,
            dynamic data = null,
            IDbTransaction transaction = null,
            string splitOn = "Id",
            int? commandTimeout = null)
        {
            var param = data as object;
            var cache = new Dictionary<TParentKey, TParent>();
            Func<TParent, TChild, TParent> parentSelector = (parent, child) =>
            {
                var parentKey = parentKeySelector(parent);
                if (!cache.ContainsKey(parentKey))
                    cache.Add(parentKey, parent);
                else
                    parent = cache[parentKey];

                var children = childSelector(parent);
                if (!children.Contains(child))
                    children.Add(child);

                return parent;
            };

            connection.Query<TParent, TChild, TParent>(sql, parentSelector, param, transaction, true, splitOn, commandTimeout);

            return cache.Values;
        }
    }
}