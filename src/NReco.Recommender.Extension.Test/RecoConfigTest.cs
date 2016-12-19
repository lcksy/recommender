using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CQSS.Common.Infrastructure.Engine;
using NReco.Recommender.Extension.Configuration;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;
using NReco.Recommender.Extension.Recommender.DataReaderResolver;

namespace NReco.Recommender.Extension.Test
{
    [TestClass]
    public class RecoConfigTest
    {
        public RecoConfigTest()
        {
            EngineContext.Initialize(true);
        }

        [TestMethod]
        public void TestResolveNRecoConfig()
        {
            var configs = NRecoConfigResolver.Resolve<NRecoConfig>();

            Assert.AreEqual(4, configs.ServerNodes.Count());
        }

        [TestMethod]
        public void TestRead()
        {
            var reader = EngineContext.Current.Resolve<IDataReaderResolver>("sqlDataReader");

            var frequencies = reader.Read();
        }

        [TestMethod]
        public void TestWrite_Update()
        {
            var reader = EngineContext.Current.Resolve<IDataReaderResolver>("sqlDataReader");

            var frequency = new ProductFrequency()
            {
                SysNo = 1,
                CustomerSysNo = 1,
                ProductSysNo = 1,
                BuyFrequency = 1.2F,
                ClickFrequency = 2.1F,
                CommentFrequency = 3.2F,
                TimeStamp = 1234567
            };

            var res = reader.Write(frequency);

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void TestWrite_Insert()
        {
            var reader = EngineContext.Current.Resolve<IDataReaderResolver>("sqlDataReader");

            var frequency = new ProductFrequency()
            {
                SysNo = 1,
                CustomerSysNo = 2,
                ProductSysNo = 2,
                BuyFrequency = 1.2F,
                ClickFrequency = 2.1F,
                CommentFrequency = 3.2F,
                TimeStamp = 1234567
            };

            var res = reader.Write(frequency);

            Assert.AreEqual(true, res);
        }
    }
}