using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public interface IDataModelPropertyDescriptor : IDataModelMemberDescriptor
    {
        int PrimaryKeyIndex { get; }
        bool PrimaryKey { get; }
        bool IsCalculated { get; }
        bool IsAutoIncrement { get; }
        bool NotMapped { get; }
        bool NotUpdatable { get; }
        string ColumnName { get; }
        string DbTypeName { get; }
        int DbTypeCode { get; }
        DbType DbType { get; }
    }
}