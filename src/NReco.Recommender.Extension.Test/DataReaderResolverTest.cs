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
            var frequency = DataReaderResolverFactory.Create().Read();

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
                BuyFrequency = 0.1953F,
                ClickFrequency = 0.25F,
                CommentFrequency = 0.5364F,
                TimeStamp = 1395859665
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
                BuyFrequency = 0.1953123F,
                ClickFrequency = 0.25123F,
                CommentFrequency = 0.5364123F,
                TimeStamp = 1395859789
            };

            var actual = DataReaderResolverFactory.Create().Write(frequency);

            Assert.IsTrue(actual);
        }
    }
}
