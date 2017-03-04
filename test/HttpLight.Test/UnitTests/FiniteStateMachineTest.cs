using System;
using System.Collections.Generic;
using System.Threading;
#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif
using HttpLight.Utils;
using NUnit.Framework;

namespace HttpLight.Test.UnitTests
{
    [TestFixture]
    public class FiniteStateMachineTest
    {
        [Test]
        public void SimpleStateMachine()
        {
            var stateMachine = new SimpleStateMachine();
            var context = new List<int>();
            stateMachine.Start(context);
            CollectionAssert.AreEqual(new[] {0, 1, 2}, context);
        }

        [Test]
        public void TwoParallelContexts()
        {
            var stateMachine = new SimpleStateMachine();
            var context1 = new List<int>();
            var context2 = new List<int>();
            var thread1 = new Thread(() => stateMachine.Start(context1));
            var thread2 = new Thread(() => stateMachine.Start(context2));
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();
            CollectionAssert.AreEqual(context1, context2);
            CollectionAssert.AreEqual(new[] {0, 1, 2}, context1);
        }

        [Test]
        public void LoopingStateMachine()
        {
            var stateMachine = new LoopingStateMachine(0);
            var context = new List<int>();
            Assert.Throws<Exception>(() => stateMachine.Start(context));
        }

#if FEATURE_ASYNC
        [Test]
        public void AsyncStateMachine()
        {
            var stateMachine = new AsyncStateMachine();
            var context = new List<string>();
            stateMachine.StartAsync(context).Wait();
            CollectionAssert.AreEqual(new[] {"sync 0", "async 1", "sync 2"}, context);
        }

        [Test]
        public void LoopingStateMachineAsync()
        {
            var stateMachine = new LoopingStateMachine(1);
            var context = new List<int>();
            Assert.Throws<AggregateException>(() => stateMachine.StartAsync(context).Wait());
        }

        [Test]
        public void AsyncStateMachineAsSync()
        {
            var stateMachine = new AsyncStateMachine();
            var context = new List<string>();
            stateMachine.Start(context);
            CollectionAssert.AreEqual(new[] {"sync 0", "async 1", "sync 2"}, context);
        }
#endif
    }

    internal class SimpleStateMachine : FiniteStateMachine<int, List<int>>
    {
        protected override int FirstState
        {
            get { return 0; }
        }

        public SimpleStateMachine()
        {
            AddState(0, x =>
            {
                Thread.Sleep(100);
                x.Add(0);
                return 1;
            });
            AddState(1, x =>
            {
                x.Add(1);
                return 2;
            });
            AddState(2, x =>
            {
                x.Add(2);
                return 3;
            });
        }
    }

    internal class LoopingStateMachine : FiniteStateMachine<int, List<int>>
    {
        private int _firstState;

        protected override int FirstState
        {
            get { return 0; }
        }

        public LoopingStateMachine(int firstState)
        {
            _firstState = firstState;
            AddState(0, x => 0);
#if FEATURE_ASYNC
            AddAsyncState(1, x => Task.FromResult(1));
#endif
        }
    }

#if FEATURE_ASYNC
    internal class AsyncStateMachine : FiniteStateMachine<int, List<string>>
    {
        protected override int FirstState
        {
            get { return 0; }
        }

        public AsyncStateMachine()
        {
            AddState(0, x =>
            {
                x.Add("sync 0");
                return 1;
            });
            AddState(1, x =>
            {
                x.Add("sync 1");
                return 2;
            });
            AddState(2, x =>
            {
                x.Add("sync 2");
                return 3;
            });
            AddAsyncState(1, x =>
            {
                x.Add("async 1");
                return Task.FromResult(2);
            });
        }
    }
#endif
}
