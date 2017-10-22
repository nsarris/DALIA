using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public class IsModelOfContextAttribute : Attribute
    {
        public Type ContextType { get; private set; }
        public IsModelOfContextAttribute(Type contextType)
        {
            ContextType = contextType;
        }
    }
}