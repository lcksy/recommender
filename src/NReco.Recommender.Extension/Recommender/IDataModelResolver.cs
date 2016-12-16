using NReco.CF.Taste.Model;

namespace NReco.Recommender.Extension.Recommender
{
    public interface IDataModelResolver
    {
        IDataModel DataModelResolver(DBType type);
    }
}