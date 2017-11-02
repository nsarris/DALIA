using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Linq2db
{
    public interface ILinqToDBDataContext : IDataContextAsync
    {
        DataConnection Linq2DBDataConnection { get; }
    }

    public interface ILinqToDBDataContext<TDataConnection> : ILinqToDBDataContext
        where TDataConnection : DataConnection
    {
        new TDataConnection Linq2DBDataConnection { get; }
    }
}
