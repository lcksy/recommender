using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace CQSS.Common.Infrastructure.Cluster
{
    public class ClusterMessageFilterTable<TFilterData> : IMessageFilterTable<TFilterData>
    {
        private List<ICluster> _clusters = new List<ICluster>();
        private Dictionary<IClusterNode, TFilterData> _nodeToEndpointMap = new Dictionary<IClusterNode, TFilterData>();

        public void Add(MessageFilter key, TFilterData value)
        {
            var filter = key as ClusterMessageFilter;

            var cluster = _clusters.SingleOrDefault(c => c.ClusterName == filter.ClusterName);
            if (cluster == null)
            {
                var finder = filter.CreateClusterNodeFinder();
                cluster = new DefaultCluster(filter.ClusterName, finder);
                _clusters.Add(cluster);
            }
            cluster.Add(filter);

            _nodeToEndpointMap.Add(filter, value);
        }

        public bool GetMatchingValues(Message message, ICollection<TFilterData> results)
        {
            var foundClusters = _clusters.Where(c => c.Finder.GetType() != typeof(MutilcastStrategy) && c.Matching(message)).ToList();
            var foundMutilcastClusters = _clusters.Where(c => c.Finder.GetType() == typeof(MutilcastStrategy) && c.Matching(message)).ToList();

            if (foundClusters.Any() && foundMutilcastClusters.Any())
                throw new Exception("Matching both cluster and mutilcast cluster.");

            if (foundClusters.Any())
            {
                if (foundClusters.Count > 1)
                    throw new Exception("Matching mutil cluster.");

                var foundCluster = foundClusters.Single();
                var foundNode = foundCluster.GetNode();
                var foundFilter = foundNode as ClusterMessageFilter;
                var foundEndpoint = _nodeToEndpointMap[foundFilter];
                results.Add(foundEndpoint);

                return true;
            }
            else if (foundMutilcastClusters.Any())
            {
                if (foundMutilcastClusters.Count > 1)
                    throw new Exception("Matching mutil mutilcast cluster.");

                var foundCluster = foundMutilcastClusters.Single();
                var foundNodes = foundCluster.GetNodeRange();
                var foundFilters = foundNodes.Select(n => n as ClusterMessageFilter);
                var foundEndpoints = foundFilters.Select(f => _nodeToEndpointMap[f]);
                foreach (var foundEndpoint in foundEndpoints)
                    results.Add(foundEndpoint);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GetMatchingValues(MessageBuffer messageBuffer, ICollection<TFilterData> results)
        {
            var message = messageBuffer.CreateMessage();
            return this.GetMatchingValues(message, results);
        }

        public bool GetMatchingValue(Message message, out TFilterData value)
        {
            value = default(TFilterData);
            var endpoints = new List<TFilterData>();
            bool found = this.GetMatchingValues(message, endpoints);
            if (found) value = endpoints.First();
            return found;
        }

        public bool GetMatchingValue(MessageBuffer messageBuffer, out TFilterData value)
        {
            var message = messageBuffer.CreateMessage();
            return this.GetMatchingValue(message, out value);
        }

        public bool GetMatchingFilters(MessageBuffer messageBuffer, ICollection<MessageFilter> results)
        {
            var message = messageBuffer.CreateMessage();
            return this.GetMatchingFilters(message, results);
        }

        public bool GetMatchingFilters(Message message, ICollection<MessageFilter> results)
        {
            var foundClusters = _clusters.Where(c => c.Matching(message)).ToList();

            if (foundClusters.Count > 1)
                throw new Exception("Matching mutil cluster.");

            if (foundClusters.Count == 0)
                return false;

            foundClusters.Single().Nodes.ForEach(n => results.Add(n as ClusterMessageFilter));
            return true;
        }

        #region 不需要实现

        public bool GetMatchingFilter(MessageBuffer messageBuffer, out MessageFilter filter)
        {
            throw new NotImplementedException();
        }

        public bool GetMatchingFilter(Message message, out MessageFilter filter)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(MessageFilter key)
        {
            throw new NotImplementedException();
        }

        public ICollection<MessageFilter> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(MessageFilter key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(MessageFilter key, out TFilterData value)
        {
            throw new NotImplementedException();
        }

        public ICollection<TFilterData> Values
        {
            get { throw new NotImplementedException(); }
        }

        public TFilterData this[MessageFilter key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(KeyValuePair<MessageFilter, TFilterData> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<MessageFilter, TFilterData> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<MessageFilter, TFilterData>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<MessageFilter, TFilterData> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<MessageFilter, TFilterData>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}