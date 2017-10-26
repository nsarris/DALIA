using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public enum EntityMemberType
    {
        ValueProperty,
        CollectionProperty,
        NavigationProperty
    }

    public enum StaticColumnValueEnum
    {
        ReadWrite = 0,
        Read = 1,
        Write = 2
    }
}

