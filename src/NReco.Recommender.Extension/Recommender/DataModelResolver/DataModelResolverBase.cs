using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NReco.CF.Taste.Model;
using NReco.Recommender.Extension.Recommender.DataModelResolver;

namespace NReco.Recommender.Extension.Recommender
{
    public class DataModelResolverBase : IDataModelResolver
    {
        public IDataModel DataModelResolver(DBType type)
        {
            try
            {
                return this.DoDataModelResolver(type);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual IDataModel DoDataModelResolver(DBType type)
        {
            
            throw new NotImplementedException();
        }
    }
}