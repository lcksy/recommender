using NReco.CF.Taste.Model;

namespace NReco.Recommender.Extension.Recommender.DataModelResolver
{
    public interface IDataModelResolver
    {
        IDataModel DataModelResolver(DBType type);
    }
}