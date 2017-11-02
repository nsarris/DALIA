using System.Collections.Generic;
using Dalia;

namespace Dalia
{
    public interface IDataSources : IReadOnlyDictionary<string, IDataSource>
    { }
    public class DataSources : Dictionary<string, IDataSource>, IDataSources
    {
        public void Add(DataSource ds)
        {
            this.Add(ds.Key, ds);
        }
    }
}

