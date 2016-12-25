using System;
using System.Collections.Generic;
using System.Linq;

using NReco.CF.Taste.Impl.Model;
using NReco.CF.Taste.Impl.Neighborhood;
using NReco.CF.Taste.Impl.Recommender;
using NReco.CF.Taste.Impl.Similarity;
using NReco.Recommender.DataContract;
using NReco.Recommender.Extension.Objects.RecommenderDataModel;
using NReco.Recommender.Extension.Recommender.DataModelResolver;
using NReco.Recommender.Extension.Recommender.DataReaderResolver;
using NReco.Recommender.ServiceContract;

namespace NReco.Recommender.Service
{
    public class RecommenderService : IRecommenderService
    {
        public List<RecommenderResponse> Recommender(long customerSysNo, int[] productSysNos)
        {
            var dataModel = DataModelResolverFactory.Create().BuilderModel();

            var plusAnonymModel = new PlusAnonymousUserDataModel(dataModel);

            var preferenceArray = new GenericUserPreferenceArray(productSysNos.Length);

            var userSysNo = customerSysNo <= 0 ? PlusAnonymousUserDataModel.TEMP_USER_ID : customerSysNo;

            preferenceArray.SetUserID(0, userSysNo);

            for (int i = 0; i < productSysNos.Length; i++)
            {
                preferenceArray.SetItemID(i, productSysNos[i]);
                preferenceArray.SetValue(i, 5);
            }

            plusAnonymModel.SetTempPrefs(preferenceArray);

            var similarity = new LogLikelihoodSimilarity(plusAnonymModel);

            var neighborhood = new NearestNUserNeighborhood(20, similarity, plusAnonymModel);

            var recommender = new GenericUserBasedRecommender(plusAnonymModel, neighborhood, similarity);

            var recommendedItems = recommender.Recommend(userSysNo, 15, null);

            return recommendedItems.Select(ri => new RecommenderResponse()
            {
                ProductSysNo = ri.GetItemID(),
                Rate = ri.GetValue()
            }).ToList();
        }

        public bool RefreshByCustomerSysNo(long customerSysNo)
        {
            try
            {
                DataModelResolverFactory.Create().BuilderModelFromCustomerSysNo(customerSysNo);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RefreshByTimeStamp(long timeStamp)
        {
            try
            {
                DataModelResolverFactory.Create().BuilderModelFromTimeStamp(timeStamp);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TrainingRecommender(List<Frequency> frequencies)
        {
            foreach (var freq in frequencies)
            {
                var frequency = new ProductFrequency()
                {
                    SysNo = freq.SysNo,
                    CustomerSysNo = freq.CustomerSysNo,
                    ProductSysNo = freq.ProductSysNo,
                    ClickFrequency = freq.ClickFrequency,
                    BuyFrequency = freq.BuyFrequency,
                    CommentFrequency = freq.CommentFrequency,
                    TimeStamp = freq.TimeStamp
                };

                DataReaderResolverFactory.Create().Write(frequency);
            }

            return true;
        }
    }
}