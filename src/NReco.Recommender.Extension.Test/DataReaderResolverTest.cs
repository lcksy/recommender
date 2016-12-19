using CQSS.Common.Infrastructure.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;
using NReco.Recommender.Extension.Recommender.DataReaderResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void TestSqlServerDataReaderResolver_Read()
        {
            Action<ProductFrequency> action = p =>
            {
                Console.WriteLine(p.SysNo);
            };

            DataReaderResolverFactory.Create().Read(action);
        }
    }
}
