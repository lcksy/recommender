using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NReco.Recommender.Extension.Objects.RecommenderDataModel
{
    public class ProductFrequency
    {
        public int SysNo { get; set; }
        public int CustomerSysNo { get; set; }
        public int ProductSysNo { get; set; }
        public decimal BuyFrequency { get; set; }
        public decimal ClickFrequency { get; set; }
        public decimal CommentFrequency { get; set; }
        public int TimeSpan { get; set; }
    }
}
