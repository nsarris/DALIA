//using System;
//using Dalia.Linq2db;

//namespace Dalia.DI.Unity
//{
//    interface IUnityLinqToDBDataContextFactory : IDataContextFactory
//    {
//        new ILinqToDBDataContext Create(Type type, IDataSource dataSource);
//        ILinqToDBDataContext Create(Type type, LinqToDBDataConnection connection);
//        new T Create<T>() where T : ILinqToDBDataContext;
//        new T Create<T>(IDataSource dataSource) where T : ILinqToDBDataContext;
//        T Create<T>(LinqToDBDataConnection connection) where T : ILinqToDBDataContext;
//        new ILinqToDBDataContext Create(Type type);
//        new ILinqToDBDataContext Current(Type type);
//        new ILinqToDBDataContext Current(Type type, IDataSource dataSource);
//        new ILinqToDBDataContext Current(Type type, string key);
//        new T Current<T>() where T : ILinqToDBDataContext;
//        new T Current<T>(IDataSource dataSource) where T : ILinqToDBDataContext;
//        new T Current<T>(string key) where T : ILinqToDBDataContext;
//    }
//}