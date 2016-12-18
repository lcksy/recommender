using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CQSS.Common.Infrastructure.Engine;
using NReco.Recommender.Extension.Configuration;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;
using NReco.Recommender.Extension.Recommender.DataReaderResolver;
using System;

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

            Action<ProductFrequency> action = p => 
            {
                Console.WriteLine(p.SysNo);
            };

            reader.Read(action);
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
                BuyFrequency = 1.2M,
                ClickFrequency = 2.1M,
                CommentFrequency = 3.2M,
                TimeSpan = 1234567
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
                BuyFrequency = 1.2M,
                ClickFrequency = 2.1M,
                CommentFrequency = 3.2M,
                TimeSpan = 1234567
            };

            var res = reader.Write(frequency);

            Assert.AreEqual(true, res);
        }
    }
}