using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpLight.Utils
{
    internal static class StreamHelper
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
#if !FEATURE_STREAMCOPYTO
        private const int BufferSize = 81920;
#endif

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
            StreamConverter converter;
            encoding = encoding ?? DefaultEncoding;
            if (!_converters.TryGetValue(type, out converter))
                return new MemoryStream(encoding.GetBytes(obj.GetType().FullName));
            return converter(obj, encoding);
        }

#if !FEATURE_STREAMCOPYTO
        public static void CopyTo(this Stream stream, Stream destination)
        {
            byte[] buffer = new byte[BufferSize];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, read);
        }
#endif

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
