using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public class MongDbDataReaderResolver : DataReaderResolverBase
    {
        public override IEnumerable<ProductFrequency> Read()
        {
            throw new NotImplementedException();
        }

        protected override bool RecordExist(ProductFrequency frequency)
        {
            throw new NotImplementedException();
        }

        protected override bool RecordInsert(ProductFrequency frequency)
        {
            throw new NotImplementedException();
        }

        protected override bool RecordUpdate(ProductFrequency frequency)
        {
            throw new NotImplementedException();
        }
    }
}