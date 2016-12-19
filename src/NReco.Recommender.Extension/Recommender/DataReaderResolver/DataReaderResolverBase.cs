using System;
using System.Collections.Generic;

using NReco.Recommender.Extension.Configuration;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public abstract class DataReaderResolverBase : IDataReaderResolver
    {
        public abstract IEnumerable<ProductFrequency> Read();
        public abstract IEnumerable<ProductFrequency> ReadByCustomerSysNo(int customerSysNo);
        public abstract IEnumerable<ProductFrequency> ReadGreaterThanTimeStamp(long timeStamp);
        public bool Write(ProductFrequency frequency)
        {
            try
            {
                if (this.DoExist(frequency))
                    return this.DoUpdate(frequency);
                else
                    return this.DoInsert(frequency);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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