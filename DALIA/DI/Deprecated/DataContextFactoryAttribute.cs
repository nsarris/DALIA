using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.DI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    internal class DataContextFactoryAttribute : Attribute
    {
        public DataContextFactoryAttribute(Type forContextInterface, Type underlyingProviderFactoryType)
        {
            ContextInterface = forContextInterface;
            UnderlyingProviderFactoryType = underlyingProviderFactoryType;
        }

        public Type ContextInterface { get; }
        public Type UnderlyingProviderFactoryType { get; }
    }
}
