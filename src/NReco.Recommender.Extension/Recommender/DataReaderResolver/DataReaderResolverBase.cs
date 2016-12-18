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
            if (this.RecordExist(frequency))
                return this.RecordUpdate(frequency);
            else
                return this.RecordInsert(frequency);
        }
        protected abstract bool RecordExist(ProductFrequency frequency);
        protected abstract bool RecordInsert(ProductFrequency frequency);
        protected abstract bool RecordUpdate(ProductFrequency frequency);
        protected NRecoConfig GetNrecoConfig()
        {
            return NRecoConfigResolver.Resolve<NRecoConfig>();   
        }
    }
}