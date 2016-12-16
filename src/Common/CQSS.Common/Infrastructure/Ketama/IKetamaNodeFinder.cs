using System.Collections.Generic;

namespace CQSS.Common.Infrastructure.Ketama
{
    public interface IKetamaNodeFinder
    {
        IEnumerable<T> Find<T>() where T : IKetamaNode;
    }
}