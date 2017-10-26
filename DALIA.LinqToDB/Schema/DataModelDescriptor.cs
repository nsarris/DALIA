using Dalia.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Linq2db.Schema
{
    public class DataModelDescriptor : IDataModelDescriptor
    {
        Dictionary<string, DataModelPropertyDescriptor> props
            = new Dictionary<string, DataModelPropertyDescriptor>();
        Dictionary<string, IDataModelPropertyDescriptor> props_iface
            = new Dictionary<string, IDataModelPropertyDescriptor>();

        public bool NotSupported { get; protected set; }
        public string NotSupportedReason { get; protected set; }
        public Type EntityType { get; private set; }
        
        public IReadOnlyList<IDataModelPropertyDescriptor> Keys { get; private set; }
        public IReadOnlyList<IDataModelPropertyDescriptor> UpdatableProperties { get; private set; }
        public IReadOnlyList<IDataModelPropertyDescriptor> CalculatedProperties { get; private set; }
        public IReadOnlyDictionary<string, IDataModelPropertyDescriptor> Properties { get { return props_iface; } }
        public IReadOnlyList<ITableDescriptor> Tables { get; private set; }

        internal DataModelDescriptor(LinqToDB.Mapping.EntityDescriptor entityDescriptor)//EntityType entityType, List<TableMapping> tableMappings, Type type)
        {
            try
            {
                EntityType = entityDescriptor.ObjectType;
                var tableDescriptor = new TableDescriptor(entityDescriptor);
                
                props = entityDescriptor.Columns
                    .Select(x => new DataModelPropertyDescriptor(x, tableDescriptor))
                    //.OfType<DataModelPropertyDescriptor>()
                    //.Distinct(x => x.Name)
                    .ToDictionary(x => x.Name);

                tableDescriptor.MappedMembers = props.Values.ToList().AsReadOnly();
                Tables = new[] { tableDescriptor};

                props_iface = props.ToDictionary(x => x.Key, x => (IDataModelPropertyDescriptor)x.Value);

                Keys = props.Values.Where(x => x.PrimaryKey).OfType<IDataModelPropertyDescriptor>().ToList();
                UpdatableProperties = props.Values.Where(x => !x.NotUpdatable).Cast<IDataModelPropertyDescriptor>().ToList();
                CalculatedProperties = props.Values.Where(x => x.IsCalculated).Cast<IDataModelPropertyDescriptor>().ToList();
            }
            catch (Exception ex)
            {
                NotSupported = true;
                NotSupportedReason = "Exception: + ex.Message";
                throw ex;
            }

        }
    }
}