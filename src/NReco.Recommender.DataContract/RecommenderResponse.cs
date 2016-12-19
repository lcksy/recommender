using System.Runtime.Serialization;

namespace NReco.Recommender.DataContract
{
    [DataContract]
    public class RecommenderResponse
    {
        [DataMember]
        public long ProductSysNo { get; set; }
        [DataMember]
        public float Rate { get; set; }
    }
}