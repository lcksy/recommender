using System.Collections.Generic;
using System.ServiceModel;

using NReco.Recommender.DataContract;

namespace NReco.Recommender.ServiceContract
{
    [ServiceContract]
    public interface IRecommenderService
    {
        [OperationContract]
        List<RecommenderResponse> Recommender(long customerSysNo, int[] productSysNos);

        [OperationContract]
        bool RefreshByCustomerSysNo(int customerSysNo);

        [OperationContract]
        bool RefreshByTimeStamp(long timeStamp);
    }
}