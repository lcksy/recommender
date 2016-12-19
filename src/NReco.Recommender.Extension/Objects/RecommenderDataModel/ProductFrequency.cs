using MongoDB.Bson.Serialization.Attributes;

namespace NReco.Recommender.Extension.Objects.RecommenderDataModel
{
    [BsonIgnoreExtraElements]
    public class ProductFrequency
    {
        public int SysNo { get; set; }
        public int CustomerSysNo { get; set; }
        public int ProductSysNo { get; set; }
        public decimal BuyFrequency { get; set; }
        public decimal ClickFrequency { get; set; }
        public decimal CommentFrequency { get; set; }
        public double TimeSpan { get; set; }
    }
}