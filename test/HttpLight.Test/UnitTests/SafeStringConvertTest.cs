using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Threading;
using HttpLight.Utils;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class SafeStringConvertTest
    {
        private static IEnumerable<TestCaseData> ChangeTypeSingleValueData
        {
            get
            {
                yield return new TestCaseData("test", typeof(string)) {ExpectedResult = "test", TestName = "string"};
                yield return new TestCaseData("10", typeof(byte)) {ExpectedResult = (byte) 10, TestName = "byte"};
                yield return new TestCaseData("", typeof(byte)) {ExpectedResult = (byte) 0, TestName = "byte default"};
                yield return new TestCaseData("10", typeof(byte?)) {ExpectedResult = (byte?) 10, TestName = "byte?"};
                yield return new TestCaseData("", typeof(byte?)) {ExpectedResult = null, TestName = "byte? default"};
                yield return new TestCaseData("10", typeof(sbyte)) {ExpectedResult = (sbyte) 10, TestName = "sbyte"};
                yield return new TestCaseData("", typeof(sbyte)) {ExpectedResult = (sbyte) 0, TestName = "sbyte default"};
                yield return new TestCaseData("10", typeof(sbyte?)) {ExpectedResult = (sbyte?) 10, TestName = "sbyte?"};
                yield return new TestCaseData("", typeof(sbyte?)) {ExpectedResult = null, TestName = "sbyte? default"};
                yield return new TestCaseData("a", typeof(char)) {ExpectedResult = 'a', TestName = "char"};
                yield return new TestCaseData("ab", typeof(char)) {ExpectedResult = (char) 0, TestName = "char default"};
                yield return new TestCaseData("a", typeof(char?)) {ExpectedResult = (char?) 'a', TestName = "char?"};
                yield return new TestCaseData("ab", typeof(char?)) {ExpectedResult = null, TestName = "char? default"};
                yield return new TestCaseData("10", typeof(short)) {ExpectedResult = (short) 10, TestName = "short"};
                yield return new TestCaseData("", typeof(short)) {ExpectedResult = (short) 0, TestName = "short default"};
                yield return new TestCaseData("10", typeof(short?)) {ExpectedResult = (short?) 10, TestName = "short?"};
                yield return new TestCaseData("", typeof(short?)) {ExpectedResult = null, TestName = "short? default"};
                yield return new TestCaseData("10", typeof(ushort)) {ExpectedResult = (ushort) 10, TestName = "ushort"};
                yield return new TestCaseData("", typeof(ushort)) {ExpectedResult = (ushort) 0, TestName = "ushort default"};
                yield return new TestCaseData("10", typeof(ushort?)) {ExpectedResult = (ushort?) 10, TestName = "ushort?"};
                yield return new TestCaseData("", typeof(ushort?)) {ExpectedResult = null, TestName = "ushort? default"};
                yield return new TestCaseData("10", typeof(int)) {ExpectedResult = 10, TestName = "int"};
                yield return new TestCaseData("", typeof(int)) {ExpectedResult = 0, TestName = "int default"};
                yield return new TestCaseData("10", typeof(int?)) {ExpectedResult = (int?) 10, TestName = "int?"};
                yield return new TestCaseData("", typeof(int?)) {ExpectedResult = null, TestName = "int? default"};
                yield return new TestCaseData("10", typeof(uint)) {ExpectedResult = (uint) 10, TestName = "uint"};
                yield return new TestCaseData("", typeof(uint)) {ExpectedResult = (uint) 0, TestName = "uint default"};
                yield return new TestCaseData("10", typeof(uint?)) {ExpectedResult = (uint?) 10, TestName = "uint?"};
                yield return new TestCaseData("", typeof(uint?)) {ExpectedResult = null, TestName = "uint? default"};
                yield return new TestCaseData("10", typeof(long)) {ExpectedResult = (long) 10, TestName = "long"};
                yield return new TestCaseData("", typeof(long)) {ExpectedResult = (long) 0, TestName = "long default"};
                yield return new TestCaseData("10", typeof(long?)) {ExpectedResult = (long?) 10, TestName = "long?"};
                yield return new TestCaseData("", typeof(long?)) {ExpectedResult = null, TestName = "long? default"};
                yield return new TestCaseData("10", typeof(ulong)) {ExpectedResult = (ulong) 10, TestName = "ulong"};
                yield return new TestCaseData("", typeof(ulong)) {ExpectedResult = (ulong) 0, TestName = "ulong default"};
                yield return new TestCaseData("10", typeof(ulong?)) {ExpectedResult = (ulong?) 10, TestName = "ulong?"};
                yield return new TestCaseData("", typeof(ulong?)) {ExpectedResult = null, TestName = "ulong? default"};
                yield return new TestCaseData("10", typeof(BigInteger)) {ExpectedResult = new BigInteger(10), TestName = "BigInteger"};
                yield return new TestCaseData("", typeof(BigInteger)) {ExpectedResult = new BigInteger(0), TestName = "BigInteger default"};
                yield return new TestCaseData("10", typeof(BigInteger?)) {ExpectedResult = (BigInteger?) new BigInteger(10), TestName = "BigInteger?"};
                yield return new TestCaseData("", typeof(BigInteger?)) {ExpectedResult = null, TestName = "BigInteger? default"};
                yield return new TestCaseData("1.5", typeof(float)) {ExpectedResult = 1.5f, TestName = "float"};
                yield return new TestCaseData("", typeof(float)) {ExpectedResult = 0.0f, TestName = "float default"};
                yield return new TestCaseData("1.5", typeof(float?)) {ExpectedResult = (float?) 1.5f, TestName = "float?"};
                yield return new TestCaseData("", typeof(float?)) {ExpectedResult = null, TestName = "float? default"};
                yield return new TestCaseData("1.5", typeof(double)) {ExpectedResult = 1.5, TestName = "double"};
                yield return new TestCaseData("", typeof(double)) {ExpectedResult = 0.0, TestName = "double default"};
                yield return new TestCaseData("1.5", typeof(double?)) {ExpectedResult = (double?) 1.5, TestName = "double?"};
                yield return new TestCaseData("", typeof(double?)) {ExpectedResult = null, TestName = "double? default"};
                yield return new TestCaseData("1.5", typeof(decimal)) {ExpectedResult = 1.5m, TestName = "decimal"};
                yield return new TestCaseData("", typeof(decimal)) {ExpectedResult = 0.0m, TestName = "decimal default"};
                yield return new TestCaseData("1.5", typeof(decimal?)) {ExpectedResult = (decimal?) 1.5m, TestName = "decimal?"};
                yield return new TestCaseData("", typeof(decimal?)) {ExpectedResult = null, TestName = "decimal? default"};
                yield return new TestCaseData("true", typeof(bool)) {ExpectedResult = true, TestName = "bool string true"};
                yield return new TestCaseData("1", typeof(bool)) {ExpectedResult = true, TestName = "bool bit true"};
                yield return new TestCaseData("false", typeof(bool)) {ExpectedResult = false, TestName = "bool string false"};
                yield return new TestCaseData("0", typeof(bool)) {ExpectedResult = false, TestName = "bool bit false"};
                yield return new TestCaseData("", typeof(bool)) {ExpectedResult = false, TestName = "bool default"};
                yield return new TestCaseData("true", typeof(bool?)) {ExpectedResult = (bool?) true, TestName = "bool? string true"};
                yield return new TestCaseData("1", typeof(bool?)) {ExpectedResult = (bool?) true, TestName = "bool? bit true"};
                yield return new TestCaseData("false", typeof(bool?)) {ExpectedResult = (bool?) false, TestName = "bool? string false"};
                yield return new TestCaseData("0", typeof(bool?)) {ExpectedResult = (bool?) false, TestName = "bool? bit false"};
                yield return new TestCaseData("", typeof(bool?)) {ExpectedResult = null, TestName = "bool? default"};
                yield return new TestCaseData("3EDB8DD5-3BDC-4B2E-B9E7-4928E7212A0C", typeof(Guid)) {ExpectedResult = Guid.Parse("3EDB8DD5-3BDC-4B2E-B9E7-4928E7212A0C"), TestName = "Guid"};
                yield return new TestCaseData("", typeof(Guid)) {ExpectedResult = Guid.Empty, TestName = "Guid default"};
                yield return new TestCaseData("3EDB8DD5-3BDC-4B2E-B9E7-4928E7212A0C", typeof(Guid?)) {ExpectedResult = (Guid?) Guid.Parse("3EDB8DD5-3BDC-4B2E-B9E7-4928E7212A0C"), TestName = "Guid?"};
                yield return new TestCaseData("", typeof(Guid?)) {ExpectedResult = null, TestName = "Guid? default"};
            }
        }

        private static IEnumerable<TestCaseData> ChangeTypeArrayData
        {
            get
            {
                yield return new TestCaseData(null) {ExpectedResult = null, TestName = "Null"};
                yield return new TestCaseData((object) new string[0]) {ExpectedResult = new int[0], TestName = "Length=0"};
                yield return new TestCaseData((object) new[] {"1"}) {ExpectedResult = new[] {1}, TestName = "Length=1"};
                yield return new TestCaseData((object) new[] {"1", "2"}) {ExpectedResult = new[] {1, 2}, TestName = "Length=2"};
                yield return new TestCaseData((object) new[] {"1", ""}) {ExpectedResult = new[] {1, 0}, TestName = "With default value"};
            }
        }

        [TestCaseSource(nameof(ChangeTypeSingleValueData))]
        public object ChangeTypeSingleValue(string s, Type type)
        {
            var culture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            var result = SafeStringConvert.ChangeType(s, type);
            return result;
        }

        [Test]
        public void ChangeTypeNotSupported()
        {
            var type = typeof(object);
            Assert.Throws<Exception>(() => SafeStringConvert.ChangeType("", type), "Type " + type.Name + " is not supported");
        }

        [TestCaseSource(nameof(ChangeTypeArrayData))]
        public int[] ChangeTypeArray(string[] array)
        {
            var type = typeof(int);
            var culture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            var result = (int[]) SafeStringConvert.ChangeType(array, type);
            return result;
        }

        [TestCase(typeof(int), ExpectedResult = true, TestName = "Supported")]
        [TestCase(typeof(object), ExpectedResult = false, TestName = "Not supported")]
        public bool IsTypeSupported(Type type)
        {
            return SafeStringConvert.IsTypeSupported(type);
        }
    }
}
