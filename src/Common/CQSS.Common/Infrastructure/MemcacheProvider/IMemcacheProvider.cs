using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQSS.Common.Infrastructure.MemcacheProvider
{
    public interface IMemcacheProvider
    {
        T Get<T>(string key);

        IEnumerable<T> GetMulti<T>(IEnumerable<string> keys);

        Dictionary<string, T> GetDictionary<T>(IEnumerable<string> keys);

        bool Set<T>(string key, T value);

        bool Set<T>(string key, T value, DateTime expiresAt);

        bool Set<T>(string key, T value, TimeSpan validFor);

        bool Remove(string key);

        bool Exists(string key);
    }
}