using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Core.Common.Fsm
{
    /// <summary>
    /// Конечный автомат с поддержкой асинхронных Enter/Exit.
    /// Пока выполняется переход (Enter/Exit) — Tick не вызывается.
    /// </summary>
    public sealed class StateMachine<TContext, TStateId> : ITickSystem, IDisposable
        where TStateId : notnull
    {
        private readonly Dictionary<TStateId, IState<TContext, TStateId>> _states = new();
        private readonly CancellationTokenSource _lifetimeCts = new();

        public TContext                           Context   { get; }
        public IState<TContext, TStateId>         Current   { get; private set; }
        public TStateId                           CurrentId { get; private set; }

        private bool _isTransitioning;
        private bool _disposed;

        public StateMachine(TContext context) => Context = context;

        public StateMachine<TContext, TStateId> Add(IState<TContext, TStateId> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            _states[state.Id] = state;
            return this;
        }

        /// <summary>
        /// Запускает автомат: переходит в начальное состояние и ожидает завершения Enter.
        /// Tick не будет вызываться до завершения Enter.
        /// </summary>
        public void Start(TStateId initial)
        {
            if (!_states.TryGetValue(initial, out var st))
                throw new InvalidOperationException($"State '{initial}' not registered.");

            Current          = st;
            CurrentId        = initial;
            _isTransitioning = true;
            _ = RunEnterAsync(st);
        }

        public void Tick(float dt)
        {
            if (_disposed || _isTransitioning || Current == null) return;

            if (Current.Tick(Context, dt, out var next))
                TransitionTo(next);
        }

        public void TransitionTo(TStateId next)
        {
            if (_isTransitioning) return;
            if (EqualityComparer<TStateId>.Default.Equals(CurrentId, next)) return;
            if (!_states.ContainsKey(next))
                throw new InvalidOperationException($"State '{next}' not registered.");

            _ = RunTransitionAsync(next);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();
        }

        // ── приватные async-методы ────────────────────────────────────────────

        private async Task RunEnterAsync(IState<TContext, TStateId> state)
        {
            try
            {
                await state.Enter(Context, _lifetimeCts.Token);
            }
            catch (OperationCanceledException) { /* dispose */ }
            finally
            {
                if (!_disposed) _isTransitioning = false;
            }
        }

        private async Task RunTransitionAsync(TStateId next)
        {
            _isTransitioning = true;
            var ct = _lifetimeCts.Token;
            try
            {
                // 1. ждём окончания текущего состояния (например, анимация выхода)
                await Current.Exit(Context, ct);

                if (!_states.TryGetValue(next, out var st))
                    throw new InvalidOperationException($"State '{next}' not registered.");

                Current   = st;
                CurrentId = next;

                // 2. ждём готовности нового состояния (например, анимация входа)
                await st.Enter(Context, ct);
            }
            catch (OperationCanceledException) { /* dispose */ }
            finally
            {
                if (!_disposed) _isTransitioning = false;
            }
        }
    }
}