namespace CQSS.Common.Infrastructure.EventPattern
{
    public interface IEventHandler<T>
    {
        void Handle(T e);
    }
}