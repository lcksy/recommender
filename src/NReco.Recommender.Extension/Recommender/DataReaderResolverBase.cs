
namespace NReco.Recommender.Extension.Recommender
{
    public abstract class DataReaderResolverBase : IDataReaderResolver
    {
        public abstract bool Read(DBType type);
    }
}