using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace HttpLight.Utils
{
    /// <summary>
    /// Converts string value to instance of known type or default value. Throws exception only in case when
    /// specified type is not supported. Check if type is supported with <see cref="IsTypeSupported"/> before conversion.
    /// </summary>
    public static class SafeStringConvert
    {
        private static readonly IDictionary<Type, Converter> Converters;
        private static readonly Type[] Collections;

        static SafeStringConvert()
        {
            Converters = new Dictionary<Type, Converter>();
            Converters[typeof(string)] = ToString;
            Converters[typeof(byte)] = ToByte;
            Converters[typeof(byte?)] = ToNullableByte;
            Converters[typeof(sbyte)] = ToSByte;
            Converters[typeof(sbyte?)] = ToNullableSByte;
            Converters[typeof(char)] = ToChar;
            Converters[typeof(char?)] = ToNullableChar;
            Converters[typeof(short)] = ToInt16;
            Converters[typeof(short?)] = ToNullableInt16;
            Converters[typeof(ushort)] = ToUInt16;
            Converters[typeof(ushort?)] = ToNullableUInt16;
            Converters[typeof(int)] = ToInt32;
            Converters[typeof(int?)] = ToNullableInt32;
            Converters[typeof(uint)] = ToUInt32;
            Converters[typeof(uint?)] = ToNullableUInt32;
            Converters[typeof(long)] = ToInt64;
            Converters[typeof(long?)] = ToNullableInt64;
            Converters[typeof(ulong)] = ToUInt64;
            Converters[typeof(ulong?)] = ToNullableUInt64;
            Converters[typeof(BigInteger)] = ToBigInteger;
            Converters[typeof(BigInteger?)] = ToNullableBigInteger;
            Converters[typeof(float)] = ToSingle;
            Converters[typeof(float?)] = ToNullableSingle;
            Converters[typeof(double)] = ToDouble;
            Converters[typeof(double?)] = ToNullableDouble;
            Converters[typeof(decimal)] = ToDecimal;
            Converters[typeof(decimal?)] = ToNullableDecimal;
            Converters[typeof(bool)] = ToBoolean;
            Converters[typeof(bool?)] = ToNullableBoolean;
            Converters[typeof(Guid)] = ToGuid;
            Converters[typeof(Guid?)] = ToNullableGuid;
            Collections = new[]
            {
                typeof(IEnumerable<>),
                typeof(ICollection<>),
                typeof(IList<>)
            };
        }

        /// <summary>
        /// Checks if specified type is supported
        /// </summary>
        public static bool IsTypeSupported(Type type)
        {
            if (IsCollection(type))
                type = GetElementType(type);
            return Converters.ContainsKey(type);
        }

        /// <summary>
        /// Converts string to instance of specified type or default value
        /// </summary>
        public static object ChangeType(string s, Type type)
        {
            return ChangeType(s, type, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Converts string to instance of specified type or default value
        /// </summary>
        public static object ChangeType(string s, Type type, IFormatProvider provider)
        {
            Converter converter;
            if (!Converters.TryGetValue(type, out converter))
                throw new Exception("Type " + type.Name + " is not supported");
            return converter(s, provider);
        }

        /// <summary>
        /// Converts string array to instance of array of specified type or null
        /// </summary>
        public static object ChangeType(string[] array, Type type)
        {
            return ChangeType(array, type, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Converts string array to instance of array of specified type or null
        /// </summary>
        public static object ChangeType(string[] array, Type type, IFormatProvider provider)
        {
            if (array == null)
                return null;
            var result = Array.CreateInstance(type, array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                var item = ChangeType(array[i], type, provider);
                result.SetValue(item, i);
            }
            return result;
        }

        /// <summary>
        /// Checks if specified type is collection that can be handled
        /// </summary>
        internal static bool IsCollection(Type type)
        {
            if (type.IsArray)
                return true;
            if (type.IsGenericType)
                type = type.GetGenericTypeDefinition();
            return Collections.Contains(type);
        }

        /// <summary>
        /// Extracts element type of the collection
        /// </summary>
        internal static Type GetElementType(Type collectionType)
        {
            return collectionType.IsArray
                ? collectionType.GetElementType()
                : collectionType.GetGenericArguments()[0];
        }

        private static object ToString(string s, IFormatProvider provider)
        {
            return s;
        }

        private static object ToByte(string s, IFormatProvider provider)
        {
            byte result;
            byte.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableByte(string s, IFormatProvider provider)
        {
            byte result;
            if (!byte.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToSByte(string s, IFormatProvider provider)
        {
            sbyte result;
            sbyte.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableSByte(string s, IFormatProvider provider)
        {
            sbyte result;
            if (!sbyte.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToChar(string s, IFormatProvider provider)
        {
            char result;
            char.TryParse(s, out result);
            return result;
        }

        private static object ToNullableChar(string s, IFormatProvider provider)
        {
            char result;
            if (!char.TryParse(s, out result))
                return null;
            return result;
        }

        private static object ToInt16(string s, IFormatProvider provider)
        {
            short result;
            short.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableInt16(string s, IFormatProvider provider)
        {
            short result;
            if (!short.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToUInt16(string s, IFormatProvider provider)
        {
            ushort result;
            ushort.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableUInt16(string s, IFormatProvider provider)
        {
            ushort result;
            if (!ushort.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToInt32(string s, IFormatProvider provider)
        {
            int result;
            int.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableInt32(string s, IFormatProvider provider)
        {
            int result;
            if (!int.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToUInt32(string s, IFormatProvider provider)
        {
            uint result;
            uint.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableUInt32(string s, IFormatProvider provider)
        {
            uint result;
            if (!uint.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToInt64(string s, IFormatProvider provider)
        {
            long result;
            long.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableInt64(string s, IFormatProvider provider)
        {
            long result;
            if (!long.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToUInt64(string s, IFormatProvider provider)
        {
            ulong result;
            ulong.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableUInt64(string s, IFormatProvider provider)
        {
            ulong result;
            if (!ulong.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToBigInteger(string s, IFormatProvider provider)
        {
            BigInteger result;
            BigInteger.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableBigInteger(string s, IFormatProvider provider)
        {
            BigInteger result;
            if (!BigInteger.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToSingle(string s, IFormatProvider provider)
        {
            float result;
            float.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableSingle(string s, IFormatProvider provider)
        {
            float result;
            if (!float.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToDouble(string s, IFormatProvider provider)
        {
            double result;
            double.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableDouble(string s, IFormatProvider provider)
        {
            double result;
            if (!double.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToDecimal(string s, IFormatProvider provider)
        {
            decimal result;
            decimal.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableDecimal(string s, IFormatProvider provider)
        {
            decimal result;
            if (!decimal.TryParse(s, NumberStyles.Any, provider, out result))
                return null;
            return result;
        }

        private static object ToBoolean(string s, IFormatProvider provider)
        {
            bool result;
            if (bool.TryParse(s, out result))
                return result;
            if (s == "1")
                return true;
            return false;
        }

        private static object ToNullableBoolean(string s, IFormatProvider provider)
        {
            bool result;
            if (bool.TryParse(s, out result))
                return result;
            if (s == "1")
                return true;
            if (s == "0")
                return false;
            return null;
        }

        private static object ToGuid(string s, IFormatProvider provider)
        {
            Guid result;
            Guid.TryParse(s, out result);
            return result;
        }

        private static object ToNullableGuid(string s, IFormatProvider provider)
        {
            Guid result;
            if (!Guid.TryParse(s, out result))
                return null;
            return result;
        }
    }

    internal delegate object Converter(string s, IFormatProvider provider);
}
