using Dalia.Schema;
using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Linq2db.Schema
{
    public class DataModelMemberDescriptor : IDataModelMemberDescriptor
    {
        public string Name { get; protected set; }
        public PropertyInfoEx PropertyInfoEx { get; protected set; }
        public EntityMemberType MemberType { get; protected set; }
        public ITableDescriptor Table { get; private set; }

        public DataModelMemberDescriptor(LinqToDB.Mapping.ColumnDescriptor columnDescriptor, ITableDescriptor table)
        {
            Name = columnDescriptor.MemberName;
            Table = table;
            PropertyInfoEx = new PropertyInfoEx(columnDescriptor.MemberInfo as PropertyInfo);
            
            //var propertyInfo = entity.EntityType.GetProperty(property.Name);
            //PropertyInfoEx = new PropertyInfoEx(propertyInfo);

            //if (property.IsCollectionType)
            //    MemberType = EntityMemberType.CollectionProperty;
            ////else if (property.isna) Navigation???
            //else
            //    MemberType = EntityMemberType.ValueProperty;
        }
    }
}
