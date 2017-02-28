using System;
using System.Collections.Generic;
using System.IO;

namespace HttpLight.Utils
{
    internal static class StreamHelper
    {
        private static IDictionary<Type, StreamConverter> _converters;

        static StreamHelper()
        {
            _converters = new Dictionary<Type, StreamConverter>();
            _converters[typeof(Stream)] = FromStream;
            _converters[typeof(string)] = FromString;
            _converters[typeof(byte[])] = FromByteArray;
        }

        public static Stream ObjectToStream(object obj, ActionInfo action)
        {
            var type = obj.GetType();
            StreamConverter converter;
            if (!_converters.TryGetValue(type, out converter))
                return new MemoryStream(new byte[0]);
            return converter(obj, action);
        }

        private static Stream FromStream(object obj, ActionInfo action)
        {
            return (Stream) obj;
        }

        private static Stream FromString(object obj, ActionInfo action)
        {
            var buf = action.Encoding.GetBytes((string) obj);
            return new MemoryStream(buf);
        }

        private static Stream FromByteArray(object obj, ActionInfo action)
        {
            return new MemoryStream((byte[]) obj);
        }
    }

    internal delegate Stream StreamConverter(object obj, ActionInfo action);
}
