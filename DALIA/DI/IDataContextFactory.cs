using System;

namespace Dalia.DI
{
    public interface IDataContextFactory
    {
        IDataContextAsync Create(Type type, IDataSource dataSource);
        //IDataContextAsync Create(Type type, LinqToDBDataConnection connection);
        T Create<T>() where T : IDataContextAsync;
        T Create<T>(IDataSource dataSource) where T : IDataContextAsync;
        //T Create<T>(LinqToDBDataConnection connection) where T : IDataContextAsync;
        IDataContextAsync Create(Type type);
        IDataContextAsync Current(Type type);
        IDataContextAsync Current(Type type, IDataSource dataSource);
        IDataContextAsync Current(Type type, string key);
        T Current<T>() where T : IDataContextAsync;
        T Current<T>(IDataSource dataSource) where T : IDataContextAsync;
        T Current<T>(string key) where T : IDataContextAsync;
    }
}
