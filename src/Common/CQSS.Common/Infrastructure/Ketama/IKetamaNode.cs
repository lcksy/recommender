using System;

namespace CQSS.Common.Infrastructure.Ketama
{
    public interface IKetamaNode : IEquatable<IKetamaNode>
    {
        bool IsAlive { get; }
        bool Ping();
        void MarkAsAlive();
        void MarkAsDead();
    }
}