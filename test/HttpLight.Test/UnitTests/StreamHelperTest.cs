using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HttpLight.Utils;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class StreamHelperTest
    {
        private static IEnumerable<TestCaseData> ObjectToStreamData
        {
            get
            {
                yield return new TestCaseData(new byte[] {1, 2, 3}, null, new byte[] {1, 2, 3}) {TestName = "byte[]"};
                yield return new TestCaseData(new MemoryStream(new byte[] {1, 2, 3}), typeof(Stream), new byte[] {1, 2, 3}) {TestName = "Stream"};
                yield return new TestCaseData("test", null, Encoding.UTF8.GetBytes("test")) {TestName = "string"};
                yield return new TestCaseData(null, null, new byte[0]) {TestName = "null"};
                yield return new TestCaseData(0, typeof(int), Encoding.UTF8.GetBytes(typeof(int).FullName)) {TestName = "Not supported type"};
            }
        }

        [TestCaseSource(nameof(ObjectToStreamData))]
        public void ObjectToStream(object input, Type baseType, byte[] expectedOutput)
        {
            var stream = baseType != null
                ? StreamHelper.ObjectToStream(input, Encoding.UTF8, baseType)
                : StreamHelper.ObjectToStream(input, Encoding.UTF8);
            var bytes = StreamToBytes(stream);
            CollectionAssert.AreEqual(expectedOutput, bytes);
        }

        private byte[] StreamToBytes(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
