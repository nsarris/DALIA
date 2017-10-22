using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public interface IStaticColumnValue
    {
        string Column { get; }
        string DbTypeName { get; }
        object Value { get; }
        StaticColumnValueEnum Type { get; }
    }
}
