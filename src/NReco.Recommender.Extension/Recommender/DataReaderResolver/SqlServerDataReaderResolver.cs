using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

using CQSS.Common.Database.Dapper;
using Dapper;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;
using NReco.Recommender.Extension;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public class SqlServerDataReaderResolver : DataReaderResolverBase
    {
        public IDbConnection CreateConnection(DBDrectionType drectionType)
        {
            if (base.NRecoConfig.ServerNodes == null || base.NRecoConfig.ServerNodes.Count() == 0)
                throw new Exception("获取 ConnectionString 失败，无法在配置文件的 NRecoConfig 中找到 sql server节点");

            SqlConnection connection = null;

            if (drectionType == DBDrectionType.Read)  //todo
                connection = new SqlConnection(base.NRecoConfig.ServerNodes.First().ConnectionString);
            else
                connection = new SqlConnection(base.NRecoConfig.ServerNodes.Last().ConnectionString);

            connection.Open(); 

            return connection;
        }
        
        public override IEnumerable<ProductFrequency> Read()
        {
            var sql = @"SELECT * FROM ProductFrequency";

            using (var connection = this.CreateConnection(DBDrectionType.Read))
            {
                return connection.Query<ProductFrequency>(sql);
            }
        }

        public override IEnumerable<ProductFrequency> ReadByCustomerSysNo(long customerSysNo)
        {
            var sql = @"SELECT * FROM ProductFrequency WHERE CustomerSysNo = @CustomerSysNo";

            using (var connection = this.CreateConnection(DBDrectionType.Read))
            {
                return connection.Query<ProductFrequency>(sql, new { CustomerSysNo = customerSysNo });
            }
        }

        public override IEnumerable<ProductFrequency> ReadGreaterThanTimeStamp(long timestamp)
        {
            var sql = @"SELECT * FROM ProductFrequency WHERE TimeStamp >= @TimeStamp";

            using (var connection = this.CreateConnection(DBDrectionType.Read))
            {
                return connection.Query<ProductFrequency>(sql, new { TimeStamp = timestamp });
            }
        }

        protected override bool DoExist(ProductFrequency frequency)
        {
            var sql = "SELECT SysNo From ProductFrequency WHERE CustomerSysNo = @CustomerSysNo AND ProductSysNo = @ProductSysNo";

            using (var connection = this.CreateConnection(DBDrectionType.Read))
            {
                var sysno = connection.ExecuteScalar<int>(sql, new { CustomerSysNo = frequency.CustomerSysNo, ProductSysNo = frequency.ProductSysNo });

                return sysno > 0;
            }
        }

        protected override bool DoInsert(ProductFrequency frequency)
        {
            using (var connection = this.CreateConnection(DBDrectionType.Write))
            {
                var data = new 
                { 
                    CustomerSysNo = frequency.CustomerSysNo, 
                    ProductSysNo = frequency.ProductSysNo, 
                    BuyFrequency = frequency.BuyFrequency, 
                    ClickFrequency = frequency.ClickFrequency, 
                    CommentFrequency = frequency.CommentFrequency,
                    TimeStamp = frequency.TimeStamp 
                };

                var res = connection.Insert(data, "ProductFrequency");

                return res > 0;
            }
        }

        protected override bool DoUpdate(ProductFrequency frequency)
        {
            var sql = @"UPDATE ProductFrequency
                        SET BuyFrequency = BuyFrequency + @BuyFrequency,
                            ClickFrequency = ClickFrequency + @ClickFrequency,
                            CommentFrequency = CommentFrequency + @CommentFrequency,
                            TimeStamp = @TimeStamp
                        WHERE CustomerSysNo = @CustomerSysNo
                            AND ProductSysNo = @ProductSysNo";

            var param = new 
            {
                CustomerSysNo = frequency.CustomerSysNo,
                ProductSysNo = frequency.ProductSysNo,
                BuyFrequency = frequency.BuyFrequency, 
                ClickFrequency = frequency.ClickFrequency, 
                CommentFrequency = frequency.CommentFrequency,
                TimeStamp = frequency.TimeStamp
            };

            using (var connection = this.CreateConnection(DBDrectionType.Write))
            {
                var res = connection.ExecuteScalar<int>(sql, param);

                return res >= 0;
            }
        }
    }

    internal static class IDataReaderExtension
    {
        public static T Map<T>(this IDataReader reader)
        {
            var instance = Activator.CreateInstance<T>();

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in properties)
            {
                var value = reader[prop.Name];

                if(prop.PropertyType == typeof(float))
                    prop.SetValue(instance, float.Parse(value.ToString()));
                else
                    prop.SetValue(instance, value);
            }

            return instance;
        }
    }
}