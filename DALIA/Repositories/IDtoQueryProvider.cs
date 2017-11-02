using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Repositories
{
    public interface IDtoQueryProvider<TDataModel, TDTO> : IQueryProvider<TDTO>
        where TDataModel : class
        where TDTO : class
    {
       
    }

    public interface IDtoQueryProvider<TDataModel, TDTO, TKey> : IDtoQueryProvider<TDataModel, TDTO>, IQueryProvider<TDTO, TKey>
         where TDataModel : class
         where TDTO : class
    {
        
    }


}
