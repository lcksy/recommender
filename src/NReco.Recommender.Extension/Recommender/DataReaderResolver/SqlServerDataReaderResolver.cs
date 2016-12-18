using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

using CQSS.Common.Database.Dapper;
using Dapper;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public class SqlServerDataReaderResolver : DataReaderResolverBase
    {
        public IDbConnection CreateConnection()
        {
            var config = base.GetNrecoConfig();

            if(config.ServerNodes == null || config.ServerNodes.Count()==0)
                throw new Exception("获取 ConnectionString 失败，无法在配置文件的 NRecoConfig 中找到 sql server节点");

            var connection = new SqlConnection(config.ServerNodes.First().ConnectionString);
            connection.Open();

            return connection;
        }
        
        public override void Read(Action<ProductFrequency> action)
        {
            var sql = @"SELECT * FROM ProductFrequency";

            using (var connection = this.CreateConnection())
            using (var reader = connection.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    var frequency = reader.Map<ProductFrequency>();

                    if(action != null)
                        action.Invoke(frequency);
                }
            }
        }

        protected override bool DoExist(ProductFrequency frequency)
        {
            var sql = "SELECT SysNo From ProductFrequency WHERE CustomerSysNo = @CustomerSysNo AND ProductSysNo = @ProductSysNo";

            using (var connection = this.CreateConnection())
            {
                var sysno = connection.ExecuteScalar<int>(sql, new { CustomerSysNo = frequency.CustomerSysNo, ProductSysNo = frequency.ProductSysNo });

                return sysno > 0;
            }
        }

        protected override bool DoInsert(ProductFrequency frequency)
        {
            using (var connection = this.CreateConnection())
            {
                var data = new 
                { 
                    CustomerSysNo = frequency.CustomerSysNo, 
                    ProductSysNo = frequency.ProductSysNo, 
                    BuyFrequency = frequency.BuyFrequency, 
                    ClickFrequency = frequency.ClickFrequency, 
                    CommentFrequency = frequency.CommentFrequency, 
                    TimeSpan = frequency.TimeSpan 
                };

                var res = connection.Insert(data, "ProductFrequency");

                return res > 0;
            }
        }

        protected override bool DoUpdate(ProductFrequency frequency)
        {
            var sql = @"UPDATE ProductFrequency
                        SET BuyFrequency = BuyFrequency + @BuyFrequency
                            ClickFrequency = ClickFrequency + @ClickFrequency
                            CommentFrequency = CommentFrequency + @CommentFrequency
                            TimeSpan = DATEDIFF(SECOND,GETDATE(),'1970-01-01')
                        WHERE CustomerSysNo = @CustomerSysNo
                            AND ProductSysNo = @ProductSysNo";

            var param = new 
            {
                CustomerSysNo = frequency.CustomerSysNo,
                ProductSysNo = frequency.ProductSysNo,
                BuyFrequency = frequency.BuyFrequency, 
                ClickFrequency = frequency.ClickFrequency, 
                CommentFrequency = frequency.CommentFrequency, 
                TimeSpan = frequency.TimeSpan 
            };

            using (var connection = this.CreateConnection())
            {
                var res = connection.ExecuteScalar<int>(sql, param);

                return res > 0;
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

                prop.SetValue(instance, value);
            }

            return instance;
        }
    }
}