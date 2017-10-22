using Dalia.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Linq2db.Schema
{
    public class DataModelPropertyDescriptor : DataModelMemberDescriptor, IDataModelPropertyDescriptor
    {
        public int PrimaryKeyIndex { get; private set; }
        public bool PrimaryKey { get { return PrimaryKeyIndex >= 0; } }
        public bool IsCalculated { get; private set; }
        public bool IsAutoIncrement { get; private set; }
        public bool NotMapped { get; private set; }
        public bool NotUpdatable { get; private set; }
        public string ColumnName { get; private set; }
        public string DbTypeName { get; private set; }
        public DbType DbType { get; private set; }
        public int DbTypeCode { get; private set; }

        //Nullable
        //DefaultValue
        //

        public DataModelPropertyDescriptor(LinqToDB.Mapping.ColumnDescriptor columnDescriptor, ITableDescriptor table)
            :base(columnDescriptor, table)
            //: base(entity, table, property)
        {
            MemberType = EntityMemberType.ValueProperty;
            PrimaryKeyIndex = columnDescriptor.IsPrimaryKey ? 
                (columnDescriptor.PrimaryKeyOrder >= 0 ? columnDescriptor.PrimaryKeyOrder : 0) 
                : -1;
            IsCalculated = columnDescriptor.IsIdentity;
            IsAutoIncrement = columnDescriptor.IsIdentity;
            //NotMapped = property.notm
            NotUpdatable = !IsCalculated;
            ColumnName = columnDescriptor.ColumnName;
            DbTypeName = columnDescriptor.DbType;
            //columnDescriptor.DataType = LinqToDB.DataType.
            //columnDescriptor.
        }
    }
}
