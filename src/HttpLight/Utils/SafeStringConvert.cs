﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Threading;

namespace HttpLight.Utils
{
    internal static class SafeStringConvert
    {
        private static IDictionary<Type, Converter> _converters;

        static SafeStringConvert()
        {
            _converters = new Dictionary<Type, Converter>();
            _converters[typeof(string)] = ToString;
            _converters[typeof(byte)] = ToByte;
            _converters[typeof(byte?)] = ToNullableByte;
            _converters[typeof(sbyte)] = ToSByte;
            _converters[typeof(sbyte?)] = ToNullableSByte;
            _converters[typeof(char)] = ToChar;
            _converters[typeof(char?)] = ToNullableChar;
            _converters[typeof(short)] = ToInt16;
            _converters[typeof(short?)] = ToNullableInt16;
            _converters[typeof(ushort)] = ToUInt16;
            _converters[typeof(ushort?)] = ToNullableUInt16;
            _converters[typeof(int)] = ToInt32;
            _converters[typeof(int?)] = ToNullableInt32;
            _converters[typeof(uint)] = ToUInt32;
            _converters[typeof(uint?)] = ToNullableUInt32;
            _converters[typeof(long)] = ToInt64;
            _converters[typeof(long?)] = ToNullableInt64;
            _converters[typeof(ulong)] = ToUInt64;
            _converters[typeof(ulong?)] = ToNullableUInt64;
            _converters[typeof(BigInteger)] = ToBigInteger;
            _converters[typeof(BigInteger?)] = ToNullableBigInteger;
            _converters[typeof(float)] = ToFloat;
            _converters[typeof(float?)] = ToNullableFloat;
            _converters[typeof(double)] = ToDouble;
            _converters[typeof(double?)] = ToNullableDouble;
            _converters[typeof(decimal)] = ToDecimal;
            _converters[typeof(decimal?)] = ToNullableDecimal;
            _converters[typeof(bool)] = ToBoolean;
            _converters[typeof(bool?)] = ToNullableBoolean;
            _converters[typeof(Guid)] = ToGuid;
            _converters[typeof(Guid?)] = ToNullableGuid;
        }

        public static object ChangeType(string s, Type type)
        {
            return ChangeType(s, type, Thread.CurrentThread.CurrentCulture);
        }

        public static object ChangeType(string s, Type type, IFormatProvider provider)
        {
            Converter converter;
            if (!_converters.TryGetValue(type, out converter))
                throw new Exception("Type " + type.Name + " is not supported");
            return converter(s, provider);
        }

        public static object[] ChangeType(string[] array, Type type)
        {
            return ChangeType(array, type, Thread.CurrentThread.CurrentCulture);
        }

        public static object[] ChangeType(string[] array, Type type, IFormatProvider provider)
        {
            var result = new object[array.Length];
            for (var i = 0; i < array.Length; i++)
                result[i] = ChangeType(array[i], type, provider);
            return result;
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

        private static object ToFloat(string s, IFormatProvider provider)
        {
            float result;
            float.TryParse(s, NumberStyles.Any, provider, out result);
            return result;
        }

        private static object ToNullableFloat(string s, IFormatProvider provider)
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
