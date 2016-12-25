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
        bool RefreshByCustomerSysNo(long customerSysNo);

        [OperationContract]
        bool RefreshByTimeStamp(long timeStamp);

        [OperationContract]
        bool TrainingRecommender(List<Frequency> frequency);
    }
}