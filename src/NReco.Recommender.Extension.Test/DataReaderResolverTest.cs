using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CQSS.Common.Infrastructure.Engine;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;
using NReco.Recommender.Extension.Recommender.DataReaderResolver;

namespace NReco.Recommender.Extension.Test
{
    [TestClass]
    public class DataReaderResolverTest
    {
        public DataReaderResolverTest()
        {
            EngineContext.Initialize(true);
        }

        [TestMethod]
        public void TestDataReaderResolver_Read()
        {
            var frequency = new List<ProductFrequency>();

            Action<ProductFrequency> action = p =>
            {
                frequency.Add(p);
            };

            DataReaderResolverFactory.Create().Read(action);

            var actualLength = frequency.Count(f => f.SysNo > 0);

            Assert.IsTrue(actualLength > 0);
        }

        [TestMethod]
        public void TestDataReaderResolver_Write_Insert()
        {
            var frequency = new ProductFrequency()
            {
                SysNo = 4,
                ProductSysNo = 46668,
                CustomerSysNo = 1099559,
                BuyFrequency = 0.19530000000000M,
                ClickFrequency = 0.25000000000000M,
                CommentFrequency = 0.53640000000000M,
                TimeSpan = 1395859665
            };

            var actual = DataReaderResolverFactory.Create().Write(frequency);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestDataReaderResolver_Write_Update()
        {
            var frequency = new ProductFrequency()
            {
                SysNo = 4,
                ProductSysNo = 46668,
                CustomerSysNo = 1099559,
                BuyFrequency = 0.19531230000000M,
                ClickFrequency = 0.25123000000000M,
                CommentFrequency = 0.53641230000000M,
                TimeSpan = 1395859789
            };

            var actual = DataReaderResolverFactory.Create().Write(frequency);

            Assert.IsTrue(actual);
        }
    }
}
