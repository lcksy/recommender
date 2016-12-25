using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using NReco.Recommender.Extension.Configuration;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public abstract class DataReaderResolverBase : IDataReaderResolver
    {
        #region prop
        protected NRecoConfig NRecoConfig { get; set; }

        private BlockingCollection<ProductFrequency> _frequencyQueue;
        #endregion

        #region actor
        public DataReaderResolverBase()
        {
            this.NRecoConfig = NRecoConfigResolver.Resolve<NRecoConfig>();

            this._frequencyQueue = new BlockingCollection<ProductFrequency>();

            Task.Factory.StartNew(() => 
            {
                var frequencies = this._frequencyQueue.GetConsumingEnumerable();
                foreach (var freq in frequencies)
                {
                    try
                    {
                        if (this.DoExist(freq))
                            this.DoUpdate(freq);
                        else
                            this.DoInsert(freq);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            });
        } 
        #endregion

        public abstract IEnumerable<ProductFrequency> Read();
        public abstract IEnumerable<ProductFrequency> ReadByCustomerSysNo(long customerSysNo);
        public abstract IEnumerable<ProductFrequency> ReadGreaterThanTimeStamp(long timeStamp);
        public bool Write(ProductFrequency frequency)
        {
            return this._frequencyQueue.TryAdd(frequency);
        }
        protected abstract bool DoExist(ProductFrequency frequency);
        protected abstract bool DoInsert(ProductFrequency frequency);
        protected abstract bool DoUpdate(ProductFrequency frequency);
    }
}