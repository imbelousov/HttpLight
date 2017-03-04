using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif

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

        public static Stream ObjectToStream(object obj, Encoding encoding, Type baseType = null)
        {
            if (obj == null)
                return new MemoryStream(new byte[0]);
            var type = baseType ?? obj.GetType();
#if FEATURE_ASYNC
            if (type == typeof(Task))
                return new MemoryStream(new byte[0]);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                type = type.GetGenericArguments().Single();
#endif
            StreamConverter converter;
            if (!_converters.TryGetValue(type, out converter))
                return new MemoryStream(encoding.GetBytes(obj.GetType().FullName));
            return converter(obj, encoding);
        }

        private static Stream FromStream(object obj, Encoding encoding)
        {
            return (Stream) obj;
        }

        private static Stream FromString(object obj, Encoding encoding)
        {
            var buf = encoding.GetBytes((string) obj);
            return new MemoryStream(buf);
        }

        private static Stream FromByteArray(object obj, Encoding encoding)
        {
            return new MemoryStream((byte[]) obj);
        }
    }

    internal delegate Stream StreamConverter(object obj, Encoding encoding);
}
