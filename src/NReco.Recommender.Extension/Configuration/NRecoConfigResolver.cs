using System.Configuration;

namespace NReco.Recommender.Extension.Configuration
{
    public class NRecoConfigResolver
    {
        public static TOut Resolve<TOut>()
        {
            var config = (TOut)ConfigurationManager.GetSection("NRecoConfig");

            return config;
        }
    }
}