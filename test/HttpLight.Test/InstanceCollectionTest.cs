using System.Threading;
using HttpLight.Utils;
using NUnit.Framework;

namespace HttpLight.Test
{
    [TestFixture]
    public class InstanceCollectionTest
    {
        [Test]
        public void OneThread()
        {
            var collection = new InstanceCollection();
            var obj1 = (InstanceCollectionTestClass) collection.GetObjectForThread(typeof(InstanceCollectionTestClass));
            var obj2 = (InstanceCollectionTestClass) collection.GetObjectForThread(typeof(InstanceCollectionTestClass));
            Assert.IsTrue(ReferenceEquals(obj1, obj2));
        }

        [Test]
        public void TwoThreads()
        {
            var collection = new InstanceCollection();
            var obj2 = null as InstanceCollectionTestClass;
            var thread = new Thread(() => obj2 = (InstanceCollectionTestClass) collection.GetObjectForThread(typeof(InstanceCollectionTestClass)));
            thread.Start();
            var obj1 = (InstanceCollectionTestClass) collection.GetObjectForThread(typeof(InstanceCollectionTestClass));
            thread.Join();
            Assert.IsFalse(ReferenceEquals(obj1, obj2));
        }

        [Test]
        public void CtorWithParams()
        {
            var collection = new InstanceCollection();
            var obj = (InstanceCollectionTestClass) collection.GetObjectForThread(typeof(InstanceCollectionTestClass), new object[] {"test"});
            Assert.AreEqual("test", obj.Param);
        }
    }

    public class InstanceCollectionTestClass
    {
        public string Param { get; }

        public InstanceCollectionTestClass()
            : this(null)
        {
        }

        public InstanceCollectionTestClass(string param)
        {
            Param = param;
        }
    }
}
