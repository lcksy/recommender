using System.Collections.Generic;

namespace CQSS.Mongo.Client
{
    public class FindResult<TDocument>
    {
        public OperateStatus Status { get; set; }
        public string Message { get; set; }
        public double Interval { get; set; }
        public IEnumerable<TDocument> Documents { get; set; }
    }
}