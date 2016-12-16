using System.Threading;

namespace CQSS.Common.Infrastructure.Cluster
{
    public class RoundRobinStrategy : LoadBalanceStrategyBase
    {
        protected int _counter = 0;
        protected int _nodesCount = 0;

        protected override IClusterNode InnerFind()
        {
            int counter = _counter;
            if (IncrementCounter(counter))
            {
                var index = counter % _nodesCount;
                var node = base.Nodes[index];
                return node;
            }
            else
            {
                return InnerFind();
            }
        }

        protected override void InnerAdd(IClusterNode node)
        {
            base.InnerAdd(node);
            _nodesCount = this.Nodes.Count;
        }

        protected virtual bool IncrementCounter(int comparand)
        {
            //var replacer = comparand + 1;
            //var oldValue = Interlocked.CompareExchange(ref _counter, replacer, comparand);
            //var newValue = _counter;

            //return oldValue != newValue;

            var newValue = comparand + 1;
            return Interlocked.CompareExchange(ref _counter, newValue, comparand) == comparand;
        }
    }
}