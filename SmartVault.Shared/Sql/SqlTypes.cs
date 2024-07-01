using System;
using System.Linq;

namespace SmartVault.Shared.Data
{
    public static class SqlTypes
    {
        public static string GetSqlType(Type type)
        {

            switch (type)
            {
                case Type when type == typeof(int):
                case Type when type == typeof(long):
                case Type when type == typeof(short):
                    return "INTEGER";
                case Type when type == typeof(string):                    
                case Type when type == typeof(DateTime):
                    return "TEXT";
                case Type when type == typeof(float):
                case Type when type == typeof(double):
                case Type when type == typeof(decimal):
                    return "DECIMAL";
                default:
                    throw new NotImplementedException($"The type '{type.Name}' is not implemented in SQL");
            }
        }
    }
}
