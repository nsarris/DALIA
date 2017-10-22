using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public class StaticColumnValue : IStaticColumnValue
    {

        public string Column { get; private set; }
        public string DbTypeName { get; private set; }
        public object Value { get; private set; }
        public StaticColumnValueEnum Type { get; private set; }
        public StaticColumnValue(string Column, string DbTypeName, object Value, StaticColumnValueEnum Type = StaticColumnValueEnum.ReadWrite)
        {
            this.Column = Column;
            this.DbTypeName = DbTypeName;
            this.Value = Value;
            this.Type = Type;
        }
    }
}
