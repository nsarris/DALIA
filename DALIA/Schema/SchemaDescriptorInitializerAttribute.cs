using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Schema
{
    public class SchemaDescriptorInitializerAttribute : Attribute
    {
        public Type InitializerType { get; private set; }
        public SchemaDescriptorInitializerAttribute(Type initializerType)
        {
            InitializerType = initializerType;
        }
    }
}
