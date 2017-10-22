using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public interface IDataModelMemberDescriptor
    {
        ITableDescriptor Table { get; }
        string Name { get; }
        PropertyInfoEx PropertyInfoEx { get; }
        EntityMemberType MemberType { get; }
    }

    

    public interface IEntityCollectionPropertySchemaModel : IDataModelMemberDescriptor
    {

    }

    public interface IEntityNavigationPropertySchemaModel : IDataModelMemberDescriptor
    {

    }
}
