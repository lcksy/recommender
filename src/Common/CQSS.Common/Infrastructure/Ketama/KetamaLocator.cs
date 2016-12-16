using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;

namespace CQSS.Common.Infrastructure.Ketama
{
    public class KetamaLocator<T>
        where T : IKetamaNode
    {
        protected List<T> _nodes;
        protected ConsistentHash<T> _container;
        protected Timer _pingWorker;

        public KetamaLocator()
        {
            //todo
        }

        public KetamaLocator(IKetamaNodeFinder finder, int pingIntervalSeconds = 60)
        {
            _nodes = finder.Find<T>().ToList();

            _container = new ConsistentHash<T>();
            _container.Init(_nodes);

            InitializePingWorker(pingIntervalSeconds);
        }

        public KetamaLocator(List<T> nodes, int pingIntervalSeconds = 60)
        {
            _nodes = nodes;

            _container = new ConsistentHash<T>();
            _container.Init(_nodes);

            InitializePingWorker(pingIntervalSeconds);
        }

        protected virtual void InitializePingWorker(int pingIntervalSeconds)
        {
            if (pingIntervalSeconds > 0)
            {
                _pingWorker = new Timer();
                _pingWorker.Interval = pingIntervalSeconds * 1000;
                _pingWorker.Elapsed += (sender, e) =>
                {
                    var deadNodes = _nodes.Where(node => !node.IsAlive).ToList();
                    foreach (var node in deadNodes)
                    {
                        if (node.Ping())
                        {
                            node.MarkAsAlive();
                            _container.Add(node);
                        }
                    }
                };
                _pingWorker.Start();
            }
        }

        public virtual T GetNode(string name = "")
        {
            if (string.IsNullOrEmpty(name))
                name = Guid.NewGuid().ToString();

            T node = default(T);
            try
            {
                node = _container.GetNode(name);
            }
            catch { }

            if (node != null && !node.Ping())
            {
                this.Remove(node, false);
                node = this.GetNode();
            }

            return node;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Add(T node)
        {
            node.MarkAsAlive();
            _container.Add(node);

            if (!_nodes.Contains(node))
                _nodes.Add(node);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Remove(T node, bool removeForever = false)
        {
            if (_nodes.Contains(node))
            {
                node.MarkAsDead();
                _container.Remove(node);

                if (removeForever)
                    _nodes.Remove(node);
            }
        }
    }
}