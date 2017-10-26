using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.AdoExtensions
{
    public static class DbCommandExtensions
    {
        public static DbParameter AddParameter<T>(this DbCommand command, string parameterName, T value)
        {
            var parameter = command.CreateParameter();

            parameter.ParameterName = parameterName;
            if (value == null)
            {
                parameter.Value = System.DBNull.Value;
            }
            else
            {
                parameter.Value = (T)value;
                parameter.DbType = value.GetType().ToDbType();
            }

            command.Parameters.Add(parameter);
            return parameter;
        }

        public static void AddParameterNonNullable<T>(this DbCommand command, string parameterName, T value)
        {
            var parameter = command.CreateParameter();

            parameter.ParameterName = parameterName;
            if (value == null)
            {
                parameter.Value = default(T);
            }
            else
            {
                parameter.Value = (T)value;
                parameter.DbType = value.GetType().ToDbType();
            }

            command.Parameters.Add(parameter);
        }

    }
}

