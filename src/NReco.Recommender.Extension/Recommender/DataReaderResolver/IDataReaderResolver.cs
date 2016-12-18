using System.Collections.Generic;

using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public interface IDataReaderResolver
    {
        IEnumerable<ProductFrequency> Read();
        bool Write(ProductFrequency frequency);
    }
}