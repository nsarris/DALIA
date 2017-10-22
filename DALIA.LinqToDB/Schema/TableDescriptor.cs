using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalia.Schema;

namespace Dalia.Linq2db.Schema
{
    public class TableDescriptor : ITableDescriptor
    {
        public int Index { get; private set; }
        public string Schema { get; private set; }
        public string TableName { get; private set; }
        public string QualifiedFullName { get; private set; }
        IReadOnlyList<DataModelMemberDescriptor> mappedMembers;
        public IReadOnlyList<DataModelMemberDescriptor> MappedMembers
        {
            get => mappedMembers;
            internal set
            {
                mappedMembers = value.ToList().AsReadOnly();
                iface_members = this.MappedMembers.Cast<IDataModelPropertyDescriptor>().ToList().AsReadOnly();
                
                Properties = iface_members.OfType<IDataModelPropertyDescriptor>().ToDictionary(x => x.Name);
                Keys = iface_members.OfType<IDataModelPropertyDescriptor>().Where(x => x.PrimaryKey).ToList().AsReadOnly();
                UpdatableProperties = iface_members.OfType<IDataModelPropertyDescriptor>().Where(x => !x.NotUpdatable).ToList().AsReadOnly();
                CalculatedProperties = iface_members.OfType<IDataModelPropertyDescriptor>().Where(x => x.IsCalculated).ToList().AsReadOnly();
                StaticColumnValues = new List<IStaticColumnValue>();
            }
        }
        IReadOnlyList<IDataModelMemberDescriptor> iface_members;
        internal TableDescriptor(LinqToDB.Mapping.EntityDescriptor entityDescriptor)//, EFEntitySchemaModel.TableMapping tableMap)
        {
            this.Index = 0;
            this.Schema = entityDescriptor.SchemaName;
            this.TableName = entityDescriptor.TableName;
            //this.QualifiedFullName = "[" + Schema + "].[" + TableName + "]";
            //this.QualifiedFullName = entityDescriptor.
        }
        
        IReadOnlyList<IDataModelMemberDescriptor> ITableDescriptor.MappedMembers { get { return iface_members; } }


        public IReadOnlyDictionary<string, IDataModelPropertyDescriptor> Properties { get; private set; }
        public IReadOnlyList<IDataModelPropertyDescriptor> Keys { get; private set; }
        public IReadOnlyList<IDataModelPropertyDescriptor> UpdatableProperties { get; private set; }
        public IReadOnlyList<IDataModelPropertyDescriptor> CalculatedProperties { get; private set; }


        public IReadOnlyList<IStaticColumnValue> StaticColumnValues { get; private set; }
    }
}
