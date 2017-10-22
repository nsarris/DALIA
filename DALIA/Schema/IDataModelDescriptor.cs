using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public interface IDataModelDescriptor
    {
        Type EntityType { get; }
        IReadOnlyList<ITableDescriptor> Tables { get; }

        IReadOnlyDictionary<string, IDataModelPropertyDescriptor> Properties { get; }
        IReadOnlyList<IDataModelPropertyDescriptor> Keys { get; }
        IReadOnlyList<IDataModelPropertyDescriptor> UpdatableProperties { get; }
        IReadOnlyList<IDataModelPropertyDescriptor> CalculatedProperties { get; }
        bool NotSupported { get; }
        string NotSupportedReason { get; }
    }
}
