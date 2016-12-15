using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Collections.Generic;
using NReco.Recommender.Extension;
using NReco.Recommender.Extension.Configuration;
using NReco.Recommender.Extension.Objects.Configuration;

namespace NReco.Recommender.Extension.Test
{
    [TestClass]
    public class RecoConfigTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var configs = ConfigurationManager.GetSection("NRecoConfig") as NRecoConfig;
        }
    }
}
