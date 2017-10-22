using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALIA.DebugTest
{
    public class ICEDataConnection : LinqToDB.Data.DataConnection
    {
        public ITable<DataModel.ICE.QueryResultItem> QueryResultItems { get; set; }
    }
}
