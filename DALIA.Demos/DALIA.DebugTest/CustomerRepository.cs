using Dalia.Repositories;
using DataModel.Northwind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalia;
using DomainObjects.Repositories;
using Dalia.DomainObjects;
using Dalia.DI;
using Dalia.Mapping;

namespace DALIA.DebugTest
{

    //[DataContextAttribute]
    [DefaultRepository]
    public class CustomerQueryProvider : DataModelQueryProvider<Customer>
    {
        public CustomerQueryProvider(NorthWindDataContext context) : base(context)
        {
        }
    }

    public class CustomerRepository : Repository<Customer>
    {
        public CustomerRepository(NorthWindDataContext context) : base(context)
        {
        }
    }

    public class CustomerRepository2 : EntityQueryProvider<Customer, DomainModel.Customer>
    {
        public CustomerRepository2(IDataContextAsync context, IObjectMapper mapper) : base(context, mapper)
        {
        }
    }
}
