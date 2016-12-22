using System;
using System.Collections.Generic;

using NReco.Recommender.Extension.Configuration;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public abstract class DataReaderResolverBase : IDataReaderResolver
    {
        #region prop
        protected NRecoConfig NRecoConfig { get; set; }
        #endregion

        #region actor
        public DataReaderResolverBase()
        {
            this.NRecoConfig = NRecoConfigResolver.Resolve<NRecoConfig>();
        } 
        #endregion

        public abstract IEnumerable<ProductFrequency> Read();
        public abstract IEnumerable<ProductFrequency> ReadByCustomerSysNo(long customerSysNo);
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
    }
}