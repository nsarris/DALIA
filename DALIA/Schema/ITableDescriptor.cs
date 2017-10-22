using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public interface ITableDescriptor
    {
        int Index { get; }
        string Schema { get; }
        string TableName { get; }
        string QualifiedFullName { get; }
        IReadOnlyList<IDataModelMemberDescriptor> MappedMembers { get; }

        IReadOnlyDictionary<string, IDataModelPropertyDescriptor> Properties { get; }
        IReadOnlyList<IDataModelPropertyDescriptor> Keys { get; }
        IReadOnlyList<IDataModelPropertyDescriptor> UpdatableProperties { get; }
        IReadOnlyList<IDataModelPropertyDescriptor> CalculatedProperties { get; }

        IReadOnlyList<IStaticColumnValue> StaticColumnValues { get; }
    }
}
