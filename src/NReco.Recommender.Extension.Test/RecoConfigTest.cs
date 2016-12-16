using CQSS.Common.Infrastructure.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NReco.Recommender.Extension;
using NReco.Recommender.Extension.Configuration;
using NReco.Recommender.Extension.Objects.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace NReco.Recommender.Extension.Test
{
    [TestClass]
    public class RecoConfigTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            EngineContext.Initialize(true);

            var configs = NRecoConfigResolver.Resolve<NRecoConfig>();
        }
    }
}
