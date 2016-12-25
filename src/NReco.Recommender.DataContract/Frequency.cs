using System.Runtime.Serialization;

namespace NReco.Recommender.DataContract
{
    [DataContract]
    public class Frequency
    {
        [DataMember]
        public int SysNo { get; set; }

        [DataMember]
        public long CustomerSysNo { get; set; }

        [DataMember]
        public long ProductSysNo { get; set; }

        [DataMember]
        public float BuyFrequency { get; set; }

        [DataMember]
        public float ClickFrequency { get; set; }

        [DataMember]
        public float CommentFrequency { get; set; }

        [DataMember]
        public long TimeStamp { get; set; }
    }
}