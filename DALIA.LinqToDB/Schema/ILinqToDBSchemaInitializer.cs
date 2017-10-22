using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Linq2db.Schema
{
    public interface ILinqToDBSchemaInitializer
    {
        void Init(LinqToDB.Data.DataConnection dataConnection);
    }
}
