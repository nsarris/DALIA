using Dynamix;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dalia.AdoExtensions
{
    public static class DbConnectionExtensions
    {
        #region GetList

        public static List<object> GetList(this DbConnection con, Type type, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<object>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    l.Add(r.MapTo(type));

            return l;
        }

        public static async Task<List<object>> GetListAsync(this DbConnection con, Type type, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<object>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = await cmd.ExecuteReaderAsync())
                while (await r.ReadAsync())
                    l.Add(r.MapTo(type));

            return l;
        }

        public static List<T> GetList<T>(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<T>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    l.Add(r.MapTo<T>());

            return l;
        }

        public static async Task<List<T>> GetListAsync<T>(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<T>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = await cmd.ExecuteReaderAsync())
                while (await r.ReadAsync())
                    l.Add(r.MapTo<T>());

            return l;
        }

        #endregion

        #region GetDictionary

        public static List<IDictionary<string, object>> GetDictionary(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<IDictionary<string, object>>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    l.Add(r.ToDictionary());

            return l;
        }

        public static async Task<List<IDictionary<string, object>>> GetDictionaryAsync(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<IDictionary<string, object>>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = await cmd.ExecuteReaderAsync())
                while (await r.ReadAsync())
                    l.Add(r.ToDictionary());

            return l;
        }

        public static List<ExpandoObject> GetExpandoObjectList(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<ExpandoObject>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = cmd.ExecuteReader())
                while (r.Read())
                    l.Add(r.ToExpandoObject());

            return l;
        }

        public static async Task<List<ExpandoObject>> GetExpandoObjectListAsync(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<ExpandoObject>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = await cmd.ExecuteReaderAsync())
                while (await r.ReadAsync())
                    l.Add(r.ToExpandoObject());

            return l;
        }

        #endregion

        #region GetDynamicType

        public static List<DynamicType> GetDynamicTypeList(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<DynamicType>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = cmd.ExecuteReader())
            {
                Type t = null;
                while (r.Read())
                    l.Add(GetDynamicTypeObjectFromDataReader(r, ref t));
            }

            return l;
        }

        public static async Task<List<DynamicType>> GetDynamicTypeListAsync(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<DynamicType>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = await cmd.ExecuteReaderAsync())
            {
                Type t = null;
                while (await r.ReadAsync())
                    l.Add(GetDynamicTypeObjectFromDataReader(r, ref t));
            }

            return l;
        }

        public static List<List<DynamicType>> GetDynamicTypeListMulti(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<List<DynamicType>>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = cmd.ExecuteReader())
                while (r.HasRows)
                {
                    Type t = null;
                    var ll = new List<DynamicType>();
                    while (r.Read())
                        ll.Add(GetDynamicTypeObjectFromDataReader(r, ref t));
                    l.Add(ll);
                    r.NextResult();
                }

            return l;
        }

        public static async Task<List<List<DynamicType>>> GetDynamicTypeListMultiAsync(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<List<DynamicType>>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = await cmd.ExecuteReaderAsync())
                while (r.HasRows)
                {
                    Type t = null;
                    var ll = new List<DynamicType>();
                    while (await r.ReadAsync())
                        ll.Add(GetDynamicTypeObjectFromDataReader(r, ref t));
                    l.Add(ll);
                    await r.NextResultAsync();
                }

            return l;
        }

        #endregion

        #region ExecuteNonQuery

        public static int ExecuteNonQuery(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            using (var cmd = con.GetCommand(query, parameters, transaction))
                return cmd.ExecuteNonQuery();
        }

        public static Task<int> ExecuteNonQueryAsync(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            using (var cmd = con.GetCommand(query, parameters, transaction))
                return cmd.ExecuteNonQueryAsync();
        }

        #endregion

        #region ExecuteScalar

        public static object ExecuteScalar(this DbConnection con, Type type, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            using (var cmd = con.GetCommand(query, parameters, transaction))
                return Convert.ChangeType(cmd.ExecuteScalar(), type);
        }

        public static T ExecuteScalar<T>(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            return (T)ExecuteScalar(con, typeof(T), query, parameters, transaction);
        }

        public static object ExecuteScalar(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            return ExecuteScalar(con, typeof(object), query, parameters, transaction);
        }

        public static async Task<object> ExecuteScalarAsync(this DbConnection con, Type type, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            using (var cmd = con.GetCommand(query, parameters, transaction))
                return Convert.ChangeType(await cmd.ExecuteScalarAsync(), type);
        }

        public static async Task<T> ExecuteScalarAsync<T>(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            return (T)(await ExecuteScalarAsync(con, typeof(T), query, parameters, transaction));
        }

        public static async Task<object> ExecuteScalarAsync(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            return await ExecuteScalarAsync(con, typeof(object), query, parameters, transaction);
        }

        public static List<T> ExecuteScalarMulti<T>(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<T>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = cmd.ExecuteReader())
                while (r.HasRows)
                {
                    while (r.Read())
                    {
                        l.Add((T)Convert.ChangeType(r[0], typeof(T)));
                    }
                    r.NextResult();
                }
            return l;
        }

        public static List<object> ExecuteScalarMulti(this DbConnection con, Type type, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<object>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = cmd.ExecuteReader())
                while (r.HasRows)
                {
                    while (r.Read())
                    {
                        l.Add(Convert.ChangeType(r[0], type));
                    }
                    r.NextResult();
                }
            return l;
        }

        public static List<object> ExecuteScalarMulti(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            return ExecuteScalarMulti<object>(con, query, parameters, transaction);
        }

        public static async Task<List<T>> ExecuteScalarMultiAsync<T>(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<T>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = await cmd.ExecuteReaderAsync())
                while (r.HasRows)
                {
                    while (await r.ReadAsync())
                    {
                        l.Add((T)Convert.ChangeType(r[0], typeof(T)));
                    }
                    await r.NextResultAsync();
                }
            return l;
        }

        public static async Task<List<object>> ExecuteScalarMultiAsync(this DbConnection con, Type type, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var l = new List<object>();
            using (var cmd = con.GetCommand(query, parameters, transaction))
            using (var r = await cmd.ExecuteReaderAsync())
                while (r.HasRows)
                {
                    while (await r.ReadAsync())
                    {
                        l.Add(Convert.ChangeType(r[0], type));
                    }
                    await r.NextResultAsync();
                }
            return l;
        }

        public static async Task<object> ExecuteScalarMultiAsync(this DbConnection con, string query, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            return await ExecuteScalarMultiAsync<object>(con, query, parameters, transaction);
        }

        #endregion

        #region GetCommand



        public static DbCommand GetCommand(this DbConnection con, string sql, QueryParameters parameters = null, DbTransaction transaction = null)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.Transaction = transaction;

            //Contract has to be class or struct

            if (parameters != null)
            {
                //var i = 0;

                if (parameters.HasNamedParameters)
                {
                    foreach (var p in parameters)
                    {
                        cmd.AddParameter(p.Name, p.Value);
                    }
                }
                else
                {
                    var commandParameters = GetParametersFromCommandText(sql);
                    foreach (var cp in commandParameters)
                    {
                        if (parameters.Contains(cp.Name))
                        {
                            var p = parameters[cp.Name];
                            cmd.AddParameter(p.Name, p.Value);
                        }
                    }
                }
            }

            return cmd;
        }

        private static QueryParameters GetParametersFromCommandText(string commandText)
        {
            var p = new QueryParameters();
            var uniqueParameters = new HashSet<string>();
            foreach (Match match in Regex.Matches(commandText, @"@([^=<>\s\'\),-[\\+*/\n]]+)"))
            {
                var paramName = match.Groups[1].Value;

                if (!p.Contains(paramName))
                    p.Add(new QueryParameter(paramName));
            }
            return p;
        }

        //public static DbCommand GetCommand(this DbConnection con, string sql, object parameters = null, DbTransaction transaction = null)
        //{
        //    var cmd = con.CreateCommand();
        //    cmd.CommandText = sql;
        //    cmd.Transaction = transaction;

        //    //Contract has to be class or struct

        //    if (parameters != null)
        //    {
        //        IEnumerable<object> col = null;
        //        IDictionary<string, object> dic = null;
        //        IEnumerable<Tuple<string, object>> tuple = null;
        //        PropertyInfo[] props = null;

        //        col = parameters as IEnumerable<object>;
        //        dic = parameters as IDictionary<string, object>;
        //        tuple = parameters as IEnumerable<Tuple<string, object>>;

        //        if (col == null && dic == null && tuple == null)
        //            props = parameters.GetType().GetProperties();

        //        var i = 0;

        //        if (col != null || props != null)
        //        {
        //            var uniqueParameters = new HashSet<string>();
        //            foreach (Match match in Regex.Matches(sql, @"@([^=<>\s\'\),-[\\+*/\n]]+)"))
        //            {
        //                var paramName = match.Groups[1].Value;

        //                if (!uniqueParameters.Contains(paramName))
        //                {
        //                    if (paramName.Length > 1 && paramName[0] != '@')
        //                    {
        //                        object value = null;
        //                        bool f = false;

        //                        if (col != null && i < col.Count())
        //                        {
        //                            var item = col.ElementAt(i);
        //                            value = item;
        //                            //TODO Error if not primitive

        //                            f = true;
        //                        }
        //                        else
        //                        {
        //                            var prop = props.Where(x => StringComparer.OrdinalIgnoreCase.Compare(x.Name,paramName) == 0).FirstOrDefault();
        //                            if (prop != null)
        //                            {
        //                                value = prop.GetValue(parameters, null);
        //                                f = true;
        //                            }
        //                        }

        //                        if (f)
        //                        {
        //                            var cmdparam = cmd.AddParameter(paramName, value);
        //                            uniqueParameters.Add(paramName);
        //                        }
        //                        i++;
        //                    }
        //                }
        //            }
        //        }
        //        else if (dic != null)
        //            foreach (var p in dic)
        //                cmd.AddParameter(p.Key, p.Value);
        //        else if (tuple != null)
        //            foreach (var p in tuple)
        //                cmd.AddParameter(p.Item1, p.Item2);
        //    }

        //    return cmd;
        //}

        #endregion

        #region Private Helpers
        private static Type GetDynamicTypeFromDataReader(DbDataReader r)
        {
            var p = new List<Dynamix.DynamicTypeProperty>();
            for (int i = 0; i < r.FieldCount; i++)
                p.Add(new Dynamix.DynamicTypeProperty { Name = r.GetName(i), Type = ToNullable(r.GetFieldType(i)) });
            return Dynamix.DynamicTypeBuilder.Instance.CreateType(p, BaseType: typeof(Dynamix.DynamicType));
        }

        private static DynamicType GetDynamicTypeObjectFromDataReader(DbDataReader r, ref Type dynamicType)
        {
            if (dynamicType == null)
                dynamicType = GetDynamicTypeFromDataReader(r);

            var o = (Dynamix.DynamicType)Activator.CreateInstance(dynamicType);
            for (int i = 0; i < r.FieldCount; i++)
                o[r.GetName(i)] = r.GetValue(i) == DBNull.Value ? null : r.GetValue(i);
            return o;
        }

        public static Type ToNullable(Type type)
        {
            // Use Nullable.GetUnderlyingType() to remove the Nullable<T> wrapper if type is already nullable.
            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType != null)
                return type;

            if (type.IsValueType)
                return typeof(Nullable<>).MakeGenericType(type);
            else
                return type;
        }

        #endregion
    }
}