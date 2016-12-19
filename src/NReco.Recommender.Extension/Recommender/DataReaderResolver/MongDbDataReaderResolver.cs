using System.Collections.Generic;
using System.Linq;

using CQSS.Common.Extension;
using CQSS.Mongo.Client;
using NReco.Recommender.Extension.Extension;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public class MongDbDataReaderResolver : DataReaderResolverBase
    {
        #region prop
        public CQSSMongoClient Client { get; set; }
        #endregion

        #region actor
        public MongDbDataReaderResolver()
        {
            this.CreateConnection();
        }
        #endregion

        private void CreateConnection()
        {
            var config = base.GetNrecoConfig();

            var connectionString = config.ServerNodes.First().ConnectionString;

            var endpoint = connectionString.Split(true, ':');

            var server = endpoint.ElementAt(0).Default(s => "localhost");

            var port = endpoint.ElementAt(1).ToInt().Default(p => 27017);

            var database = endpoint.ElementAt(2).Default(t => "recommender");

            this.Client = new CQSSMongoClient(server, port, database);
        }

        public override IEnumerable<ProductFrequency> Read()
        {
            var cursor = this.Client.FindSync<ProductFrequency>("ProductFrequency", p => p.CustomerSysNo >= -1);
            while (cursor.MoveNextAsync().Result)
            {
                foreach (var product in cursor.Current)
                    yield return product;
            }
        }

        public override IEnumerable<ProductFrequency> ReadByCustomerSysNo(int customerSysNo)
        {
            var cursor = this.Client.FindSync<ProductFrequency>("ProductFrequency", p => p.CustomerSysNo == customerSysNo);
            while (cursor.MoveNextAsync().Result)
            {
                foreach (var product in cursor.Current)
                    yield return product;
            }
        }

        public override IEnumerable<ProductFrequency> ReadGreaterThanTimeStamp(long timestamp)
        {
            var cursor = this.Client.FindSync<ProductFrequency>("ProductFrequency", p => p.TimeStamp == timestamp);
            while (cursor.MoveNextAsync().Result)
            {
                foreach (var product in cursor.Current)
                    yield return product;
            }
        }

        protected override bool DoExist(ProductFrequency frequency)
        {
            var result = this.Client.Find<ProductFrequency, int>("ProductFrequency", p => p.CustomerSysNo == frequency.CustomerSysNo && p.ProductSysNo == frequency.ProductSysNo, p => p.SysNo);

            return result.Documents.Count() > 0;
        }

        protected override bool DoInsert(ProductFrequency frequency)
        {
            var result = this.Client.InsertOne<ProductFrequency>("ProductFrequency", frequency);

            return result.AffectCount > 0;
        }

        protected override bool DoUpdate(ProductFrequency frequency)
        {
            var result = this.Client.Update<ProductFrequency>("ProductFrequency", p => p.SysNo == frequency.SysNo, frequency);

            return result.AffectCount > 0;
        }
    }
}