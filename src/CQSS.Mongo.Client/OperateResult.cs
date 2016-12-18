namespace CQSS.Mongo.Client
{
    public class OperateResult
    {
        public OperateStatus Status { get; set; }
        public long AffectCount { get; set; }
        public string Message { get; set; }
        public double Interval { get; set; }

        public override string ToString()
        {
            var message = string.Format("Status = {0}, AffectCount = {1}, Interval = {2}, Message = {3}", this.Status.ToString(), this.AffectCount, this.Interval, this.Message);

            return message;
        }
    }
}