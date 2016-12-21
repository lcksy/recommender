using System;

using CQSS.Common.Infrastructure.Engine;
using NReco.Recommender.Extension.Recommender.DataModelResolver;

namespace NReco.Recommender.Service
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            EngineContext.Initialize(true);

            var dataModel = DataModelResolverFactory.Create().BuilderModel();
        }
    }
}