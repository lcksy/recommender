using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NReco.Recommender.Extension.Recommender
{
    public interface IDataReaderResolver
    {
        bool Read(DBType type);
    }
}