using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Linq2db
{
    public interface ILinqToDBDataContext<TDataConnection> : IDataContextAsync
        where TDataConnection : LinqToDB.Data.DataConnection
    {
        TDataConnection Linq2DBDataConnection { get; }
    }

    public interface ILinqToDBDataContext : ILinqToDBDataContext<LinqToDB.Data.DataConnection>
    {
    }
}
