//using Dalia.Linq2db;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Dalia.DI
//{
//    interface ILinqToDBDataConnectionFactory
//    {
//        LinqToDBDataConnection Create(Type type);
//        LinqToDBDataConnection Create(Type type, IDataSource dataSource);
//        LinqToDBDataConnection Create(Type type, IDataSource dataSource, DbConnection connection);
//        T Create<T>() where T : LinqToDBDataConnection;
//        T Create<T>(IDataSource dataSource) where T : LinqToDBDataConnection;
//        T Create<T>(IDataSource dataSource, DbConnection connection) where T : LinqToDBDataConnection;

//        LinqToDBDataConnection Current(Type type, IDataSource dataSource);
//        LinqToDBDataConnection Current(Type type, string key);
//        LinqToDBDataConnection Current(Type type);
//        T Current<T>(IDataSource dataSource) where T : LinqToDBDataConnection;
//        T Current<T>(string key) where T : LinqToDBDataConnection;
//        T Current<T>() where T : LinqToDBDataConnection;
//    }
//}
