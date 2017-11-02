using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.DI
{
    public class DefaultDataSourceAttribute : Attribute
    {
        public DefaultDataSourceAttribute(string key)
        {
            this.Key = key;
        }

        public DefaultDataSourceAttribute(object key)
        {
            this.Key = key.ToString();
        }

        public string Key { get; }
    }
}
