using System;
using System.Collections.Generic;

#if FEATURE_ASYNC
using System.Threading.Tasks;
#endif

namespace HttpLight.Utils
{
    internal abstract class FiniteStateMachine<TState, TContext>
    {
        private const int InfiniteLoop = 5;

        private IDictionary<TState, Func<TContext, TState>> _actions;
#if FEATURE_ASYNC
        private IDictionary<TState, Func<TContext, Task<TState>>> _asyncActions;
#endif

        protected abstract TState FirstState { get; }

        public FiniteStateMachine()
        {
            _actions = new Dictionary<TState, Func<TContext, TState>>();
#if FEATURE_ASYNC
            _asyncActions = new Dictionary<TState, Func<TContext, Task<TState>>>();
#endif
        }

        protected void AddState(TState state, Func<TContext, TState> action)
        {
            _actions[state] = action;
        }

#if FEATURE_ASYNC
        protected void AddAsyncState(TState state, Func<TContext, Task<TState>> asyncAction)
        {
            _asyncActions[state] = asyncAction;
        }
#endif

        public void Start(TContext context)
        {
            var state = FirstState;
            var loopCounter = 0;
            while (true)
            {
                var prevState = state;
                Func<TContext, TState> action;
#if FEATURE_ASYNC
                Func<TContext, Task<TState>> asyncAction;
                if (_asyncActions.TryGetValue(state, out asyncAction))
                    state = Task.Run(() => asyncAction(context)).Result;
                else
#endif
                if (_actions.TryGetValue(state, out action))
                    state = action(context);
                else
                    break;
                if (state.Equals(prevState))
                    loopCounter++;
                else
                    loopCounter = 0;
                if (loopCounter > InfiniteLoop)
                    throw new Exception("Infinite loop detected");
            }
        }

#if FEATURE_ASYNC
        public async Task StartAsync(TContext context)
        {
            var state = FirstState;
            var loopCounter = 0;
            while (true)
            {
                var prevState = state;
                Func<TContext, TState> action;
                Func<TContext, Task<TState>> asyncAction;
                if (_asyncActions.TryGetValue(state, out asyncAction))
                    state = await asyncAction(context);
                else if (_actions.TryGetValue(state, out action))
                    state = action(context);
                else
                    break;
                if (state.Equals(prevState))
                    loopCounter++;
                else
                    loopCounter = 0;
                if (loopCounter > InfiniteLoop)
                    throw new Exception("Infinite loop detected");
            }
        }
#endif
    }
}
