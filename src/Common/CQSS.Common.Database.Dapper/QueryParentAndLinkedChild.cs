using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;

namespace CQSS.Common.Database.Dapper
{
    /// <summary>
    /// 调用示例 Hardway.Common.Database.Dapper.Test.SqlMapperExtensionTest 类的 Database_Dapper_QueryParentAndLinkedChild 方法
    /// </summary>
    public static partial class DapperExtension
    {
        /// <summary>
        /// 获取【父实体】包含【子实体列表】，【子实体】又包含【子实体的子实体列表】
        /// </summary>
        public static IEnumerable<TParent> QueryParentAndLinkedChild<TParent, TParentKey, TChild, TChildKey, TChildChild>(
            this IDbConnection connection,
            string sql,
            Func<TParent, TParentKey> parentKeySelector,
            Func<TParent, IList<TChild>> childSelector,
            Func<TChild, TChildKey> childKeySelector,
            Func<TChild, IList<TChildChild>> childChildSelector,
            dynamic param = null,
            IDbTransaction transaction = null,
            string splitOn = "Id",
            int? commandTimeout = null)
        {
            Dictionary<TParentKey, TParent> parentCache = new Dictionary<TParentKey, TParent>();
            Dictionary<TChildKey, TChild> childCache = new Dictionary<TChildKey, TChild>();

            connection.Query<TParent, TChild, TChildChild, TParent>(
                sql,
                (parent, child, childChild) =>
                {
                    if (!parentCache.ContainsKey(parentKeySelector(parent)))
                        parentCache.Add(parentKeySelector(parent), parent);

                    TParent cachedParent = parentCache[parentKeySelector(parent)];

                    IList<TChild> children = childSelector(cachedParent);
                    TChildKey childKey = childKeySelector(child);
                    if (!childCache.ContainsKey(childKey))
                    {
                        children.Add(child);
                        childCache.Add(childKey, child);
                    }

                    IList<TChildChild> childChildren = childChildSelector(childCache[childKey]);
                    childChildren.Add(childChild);

                    return cachedParent;
                },
                param as object, transaction, true, splitOn, commandTimeout);

            return parentCache.Values;
        }
    }
}