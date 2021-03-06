﻿using System.Collections.Generic;

using NReco.Recommender.Extension.Objects.RecommenderDataModel;

namespace NReco.Recommender.Extension.Recommender.DataReaderResolver
{
    public interface IDataReaderResolver
    {
        IEnumerable<ProductFrequency> Read();
        IEnumerable<ProductFrequency> ReadByCustomerSysNo(long customerSysNo);
        IEnumerable<ProductFrequency> ReadGreaterThanTimeStamp(long timeStamp);
        bool Write(ProductFrequency frequency);
    }
}