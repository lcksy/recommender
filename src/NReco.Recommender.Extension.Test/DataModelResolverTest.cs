using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CQSS.Common.Infrastructure.Engine;
using NReco.Recommender.Extension.Recommender.DataModelResolver;
using NReco.CF.Taste.Model;

namespace NReco.Recommender.Extension.Test
{
    [TestClass]
    public class DataModelResolverTest
    {
        public DataModelResolverTest()
        {
            EngineContext.Initialize(true);
        }

        [TestMethod]
        public void TestSqlServerDataModelResolver()
        {
            var resolver = new SqlServerDataModelResolver();

            IDataModel model = resolver.BuilderModel();

            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void TestMongoDbDataModelResolver()
        {
            var resolver = new MongoDbDataModelResolver();

            IDataModel model = resolver.BuilderModel();

            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void TestDataModelResolverFactory()
        {
            var model = DataModelResolverFactory.Create().BuilderModel();

            Assert.IsNotNull(model);
        }
    }
}
