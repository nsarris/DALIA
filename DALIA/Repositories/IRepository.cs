using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Repositories
{
    public interface IRepository : IQueryProvider { }

    public interface IRepository<T> : IQueryProvider<T> where T : class
    {
        void Insert(T obj);
        void Update(T obj);
        void Delete(T obj);
        void DeleteById(object id);

        Task InsertAsync(T obj);
        Task UpdateAsync(T obj);
        Task DeleteAsync(T obj);
        Task DeleteByIdAsync(object id);
    }

    public interface IRepository<T, TKey> : IRepository<T>, IQueryProvider<T, TKey> where T : class
    {
        void DeleteById(TKey id);
        Task DeleteByIdAsync(TKey id);
    }

    public interface IMappedRepository<TDataModel,TDTO> : IRepository<TDTO>, IDtoQueryProvider<TDataModel, TDTO> 
        where TDataModel : class
        where TDTO : class
    {
        void Insert(TDataModel obj);
        void Update(TDataModel obj);
        void Delete(TDataModel obj);

        Task InsertAsync(TDataModel obj);
        Task UpdateAsync(TDataModel obj);
        Task DeleteAsync(TDataModel obj);
    }

    public interface IMappedRepository<TDataModel, TDTO, TKey> : IMappedRepository<TDataModel, TDTO>, IDtoQueryProvider<TDataModel, TDTO, TKey>
        where TDataModel : class
        where TDTO : class
    {
        void DeleteById(TKey id);
        Task DeleteByIdAsync(TKey id);
    }
}
