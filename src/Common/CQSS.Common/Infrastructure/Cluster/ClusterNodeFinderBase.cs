using System.Collections.Generic;
using System.Threading;

namespace CQSS.Common.Infrastructure.Cluster
{
    public abstract class ClusterNodeFinderBase : IClusterNodeFinder
    {
        protected ReaderWriterLockSlim Locker { get; set; }
        protected List<IClusterNode> Nodes { get; set; }

        public ClusterNodeFinderBase()
        {
            this.Locker = new ReaderWriterLockSlim();
            this.Nodes = new List<IClusterNode>();
        }

        protected abstract IClusterNode InnerFind();

        protected virtual IEnumerable<IClusterNode> InnerFindRange()
        {
            return this.Nodes;
        }

        protected virtual void InnerAdd(IClusterNode node)
        {
            this.Nodes.Add(node);
        }

        public IClusterNode Find()
        {
            if (this.Nodes.Count == 0)
                return null;

            this.Locker.EnterReadLock();
            try
            {
                return this.InnerFind();
            }
            finally
            {
                this.Locker.ExitReadLock();
            }
        }

        public IEnumerable<IClusterNode> FindRange()
        {
            if (this.Nodes.Count == 0)
                return null;

            this.Locker.EnterReadLock();
            try
            {
                return this.InnerFindRange();
            }
            finally
            {
                this.Locker.ExitReadLock();
            }
        }

        public void Add(IClusterNode node)
        {
            this.Locker.EnterWriteLock();
            try
            {
                this.InnerAdd(node);
            }
            finally
            {
                this.Locker.ExitWriteLock();
            }
        }
    }
}