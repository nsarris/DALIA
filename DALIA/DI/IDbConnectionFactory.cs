using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.DI
{
    public interface IDbConnectionFactory
    {
        DbConnection Create(IDataSource dataSource);
        DbConnection Create(Type type, string connectionString);
        T Create<T>(string connectionString) where T : DbConnection;
        DbConnection Current(IDataSource dataSource);
        DbConnection Current(string key);
        T Current<T>() where T : DbConnection;
        T Current<T>(string key) where T : DbConnection;
    }
}
