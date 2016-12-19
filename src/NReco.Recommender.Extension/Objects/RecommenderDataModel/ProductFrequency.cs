using MongoDB.Bson.Serialization.Attributes;

namespace NReco.Recommender.Extension.Objects.RecommenderDataModel
{
    [BsonIgnoreExtraElements]
    public class ProductFrequency
    {
        public int SysNo { get; set; }
        public long CustomerSysNo { get; set; }
        public long ProductSysNo { get; set; }
        public float BuyFrequency { get; set; }
        public float ClickFrequency { get; set; }
        public float CommentFrequency { get; set; }
        public long TimeStamp { get; set; }
    }
}