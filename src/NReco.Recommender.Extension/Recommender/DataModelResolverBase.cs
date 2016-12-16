using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NReco.Recommender.Extension.Recommender
{
    public class DataModelResolverBase : IDataModelResolver
    {
        public CF.Taste.Model.IDataModel DataModelResolver(DBType type)
        {
            throw new NotImplementedException();
        }
    }
}