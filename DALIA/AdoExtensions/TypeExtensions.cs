using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.AdoExtensions
{
    public static class TypeExtensions
    {
        private static Dictionary<Type, DbType> _typeToDbTypeMap;
        private static Dictionary<DbType, Type> _dbTypeToTypeMap;
        private static Dictionary<DbType, Type> _dbTypeToNullableTypeMap;

        private static Dictionary<Type, DbType> TypeToDbTypeMap { get { CreateDbTypeMaps(); return _typeToDbTypeMap; } }
        private static Dictionary<DbType, Type> DbTypeToTypeMap { get { CreateDbTypeMaps(); return _dbTypeToTypeMap; } }
        private static Dictionary<DbType, Type> DbTypeToNullableTypeMap { get { CreateDbTypeMaps(); return _dbTypeToNullableTypeMap; } }

        private static void CreateDbTypeMaps()
        {
            if (_typeToDbTypeMap == null)
            {
                _typeToDbTypeMap = new Dictionary<Type, DbType>();
                _typeToDbTypeMap[typeof(byte)] = DbType.Byte;
                _typeToDbTypeMap[typeof(sbyte)] = DbType.SByte;
                _typeToDbTypeMap[typeof(short)] = DbType.Int16;
                _typeToDbTypeMap[typeof(ushort)] = DbType.UInt16;
                _typeToDbTypeMap[typeof(int)] = DbType.Int32;
                _typeToDbTypeMap[typeof(uint)] = DbType.UInt32;
                _typeToDbTypeMap[typeof(long)] = DbType.Int64;
                _typeToDbTypeMap[typeof(ulong)] = DbType.UInt64;
                _typeToDbTypeMap[typeof(float)] = DbType.Single;
                _typeToDbTypeMap[typeof(double)] = DbType.Double;
                _typeToDbTypeMap[typeof(decimal)] = DbType.Decimal;
                _typeToDbTypeMap[typeof(bool)] = DbType.Boolean;
                _typeToDbTypeMap[typeof(string)] = DbType.String;
                _typeToDbTypeMap[typeof(char)] = DbType.String;
                _typeToDbTypeMap[typeof(Guid)] = DbType.Guid;
                _typeToDbTypeMap[typeof(DateTime)] = DbType.DateTime;
                //_typeToDbTypeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
                _typeToDbTypeMap[typeof(TimeSpan)] = DbType.Time;
                _typeToDbTypeMap[typeof(byte[])] = DbType.Binary;
                _typeToDbTypeMap[typeof(byte?)] = DbType.Byte;
                _typeToDbTypeMap[typeof(short?)] = DbType.Int16;
                _typeToDbTypeMap[typeof(ushort?)] = DbType.UInt16;
                _typeToDbTypeMap[typeof(int?)] = DbType.Int32;
                _typeToDbTypeMap[typeof(uint?)] = DbType.UInt32;
                _typeToDbTypeMap[typeof(long?)] = DbType.Int64;
                _typeToDbTypeMap[typeof(ulong?)] = DbType.UInt64;
                _typeToDbTypeMap[typeof(float?)] = DbType.Single;
                _typeToDbTypeMap[typeof(double?)] = DbType.Double;
                _typeToDbTypeMap[typeof(decimal?)] = DbType.Decimal;
                _typeToDbTypeMap[typeof(bool?)] = DbType.Boolean;
                _typeToDbTypeMap[typeof(char?)] = DbType.String;
                _typeToDbTypeMap[typeof(Guid?)] = DbType.Guid;
                _typeToDbTypeMap[typeof(DateTime?)] = DbType.DateTime;
                //_typeToDbTypeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
                _typeToDbTypeMap[typeof(TimeSpan?)] = DbType.Time;
                //typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
            }

            if (_dbTypeToTypeMap == null)
            {
                _dbTypeToTypeMap = new Dictionary<DbType, Type>();

                _dbTypeToTypeMap[DbType.Byte] = typeof(byte);
                _dbTypeToTypeMap[DbType.SByte] = typeof(sbyte);
                _dbTypeToTypeMap[DbType.Int16] = typeof(short);
                _dbTypeToTypeMap[DbType.UInt16] = typeof(ushort);
                _dbTypeToTypeMap[DbType.Int32] = typeof(int);
                _dbTypeToTypeMap[DbType.UInt32] = typeof(uint);
                _dbTypeToTypeMap[DbType.Int64] = typeof(long);
                _dbTypeToTypeMap[DbType.UInt64] = typeof(ulong);
                _dbTypeToTypeMap[DbType.Single] = typeof(float);
                _dbTypeToTypeMap[DbType.Double] = typeof(double);
                _dbTypeToTypeMap[DbType.Decimal] = typeof(decimal);
                _dbTypeToTypeMap[DbType.Boolean] = typeof(bool);
                _dbTypeToTypeMap[DbType.String] = typeof(string);
                _dbTypeToTypeMap[DbType.StringFixedLength] = typeof(string);
                _dbTypeToTypeMap[DbType.Guid] = typeof(Guid);
                _dbTypeToTypeMap[DbType.DateTime] = typeof(DateTime);
                //_dbTypeToTypeMap[DbType.DateTimeOffset] = typeof(DateTimeOffset);
                _dbTypeToTypeMap[DbType.Binary] = typeof(byte[]);
            }

            if (_dbTypeToNullableTypeMap == null)
            {
                _dbTypeToNullableTypeMap = new Dictionary<DbType, Type>();

                _dbTypeToNullableTypeMap[DbType.Byte] = typeof(byte?);
                _dbTypeToNullableTypeMap[DbType.SByte] = typeof(sbyte?);
                _dbTypeToNullableTypeMap[DbType.Int16] = typeof(short?);
                _dbTypeToNullableTypeMap[DbType.UInt16] = typeof(ushort?);
                _dbTypeToNullableTypeMap[DbType.Int32] = typeof(int?);
                _dbTypeToNullableTypeMap[DbType.UInt32] = typeof(uint?);
                _dbTypeToNullableTypeMap[DbType.Int64] = typeof(long?);
                _dbTypeToNullableTypeMap[DbType.UInt64] = typeof(ulong?);
                _dbTypeToNullableTypeMap[DbType.Single] = typeof(float?);
                _dbTypeToNullableTypeMap[DbType.Double] = typeof(double?);
                _dbTypeToNullableTypeMap[DbType.Decimal] = typeof(decimal?);
                _dbTypeToNullableTypeMap[DbType.Boolean] = typeof(bool?);
                _dbTypeToNullableTypeMap[DbType.String] = typeof(string);
                _dbTypeToNullableTypeMap[DbType.StringFixedLength] = typeof(string);
                _dbTypeToNullableTypeMap[DbType.Guid] = typeof(Guid?);
                _dbTypeToNullableTypeMap[DbType.DateTime] = typeof(DateTime?);
                //_dbTypeToNullableTypeMap[DbType.DateTimeOffset] = typeof(DateTimeOffset?);
                _dbTypeToNullableTypeMap[DbType.Binary] = typeof(byte[]);
            }

        }

        public static DbType ToDbType(this Type type)
        {
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            if (TypeToDbTypeMap.ContainsKey(type))
                return TypeToDbTypeMap[type];
            else
                return DbType.String;
        }

        public static Type ToType(this DbType type)
        {
            if (DbTypeToTypeMap.ContainsKey(type))
                return DbTypeToTypeMap[type];
            else
                return typeof(string);
        }

        public static Type ToNullableType(this DbType dbType)
        {
            if (DbTypeToNullableTypeMap.ContainsKey(dbType))
                return DbTypeToNullableTypeMap[dbType];
            else
                return typeof(string);
        }


        public static bool IsDbConvertible(this Type type)
        {
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            return TypeToDbTypeMap.ContainsKey(type);
        }
    }
}
