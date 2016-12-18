using System;

using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public interface IDataReaderResolver
    {
        void Read(Action<ProductFrequency> action);
        bool Write(ProductFrequency frequency);
    }
}