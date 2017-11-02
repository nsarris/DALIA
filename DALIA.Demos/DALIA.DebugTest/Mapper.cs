using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoMapperExtensions;

namespace DALIA.DebugTest
{
    public class CustomerMapperConfig : IAutoMapperConfigurator
    {
        public void ConfigAuto(IProfileExpression cfg)
        {
            cfg.CreateMap<DataModel.Northwind.Customer, DomainModel.Customer>();
        }

        public void ConfigManual(IProfileExpression cfg)
        {
            cfg.FromExpression(
                (DataModel.Northwind.Customer c) => new DomainModel.Customer
                {
                    CustomerID = c.CustomerID,
                    Address = c.Orders.Count().ToString()
                });
        }
    }

    public class Mapper : Dalia.Mapping.IObjectMapper
    {
        AutoMapper.IMapper mapper;
        public Mapper(AutoMapper.IMapper mapper)
        {
            this.mapper = mapper;
        }
        public Expression<Func<TSource, TTarget>> GetMapExpression<TSource, TTarget>()
        {
            return mapper.GetExpression<TSource, TTarget>();
        }

        public Func<TSource, TTarget> GetMapFunction<TSource, TTarget>()
        {
            return GetMapExpression<TSource, TTarget>().Compile();
        }

        public TTarget MapTo<TSource, TTarget>(TSource source)
        {
            return mapper.Map<TSource, TTarget>(source);
        }
    }
}
