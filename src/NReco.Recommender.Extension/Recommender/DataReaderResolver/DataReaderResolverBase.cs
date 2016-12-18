using System.Collections.Generic;

using NReco.Recommender.Extension.Configuration;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public abstract class DataReaderResolverBase : IDataReaderResolver
    {
        public abstract IEnumerable<ProductFrequency> Read();
        public bool Write(ProductFrequency frequency)
        {
            if (this.DoExist(frequency))
                return this.DoUpdate(frequency);
            else
                return this.DoInsert(frequency);
        }
        protected abstract bool DoExist(ProductFrequency frequency);
        protected abstract bool DoInsert(ProductFrequency frequency);
        protected abstract bool DoUpdate(ProductFrequency frequency);
        protected NRecoConfig GetNrecoConfig()
        {
            return NRecoConfigResolver.Resolve<NRecoConfig>();   
        }
    }
}