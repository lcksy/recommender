using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NReco.Recommender.Extension.Recommender.DataReaderResolver;
using CQSS.Common.Infrastructure.Engine;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Test
{
    [TestClass]
    public class MongoDbDataReaderResolverTest
    {
        public MongoDbDataReaderResolverTest()
        {
            EngineContext.Initialize(true);
        }

        [TestMethod]
        public void TestResolveNRecoConfig()
        {
            Action<ProductFrequency> action = p => 
            {
                Console.WriteLine(p.SysNo);
            };

            DataReaderResolverFactory.Create().Read(action);
        }
    }
}