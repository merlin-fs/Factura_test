using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Core.Common.Fsm
{
    /// <summary>
    /// Скінченний автомат із підтримкою асинхронних методів Enter/Exit.
    /// Під час переходу між станами Tick не викликається.
    /// </summary>
    public sealed class StateMachine<TContext, TStateId> : ITickSystem, IDisposable
        where TStateId : notnull
    {
        private readonly Dictionary<TStateId, IState<TContext, TStateId>> _states = new();
        private readonly CancellationTokenSource _lifetimeCts = new();

        /// <summary>Контекст, що передається всім станам.</summary>
        public TContext                           Context   { get; }
        /// <summary>Поточний активний стан.</summary>
        public IState<TContext, TStateId>         Current   { get; private set; }
        /// <summary>Ідентифікатор поточного стану.</summary>
        public TStateId                           CurrentId { get; private set; }

        private bool     _isTransitioning;
        private bool     _disposed;
        private bool     _hasForcedTransition;
        private TStateId _forcedNextId;

        /// <summary>
        /// Створює автомат із заданим контекстом.
        /// </summary>
        /// <param name="context">Контекст для всіх станів.</param>
        public StateMachine(TContext context) => Context = context;

        /// <summary>
        /// Додає стан до автомата.
        /// </summary>
        /// <param name="state">Стан для додавання.</param>
        /// <returns>Посилання на поточний автомат для ланцюгового виклику.</returns>
        public StateMachine<TContext, TStateId> Add(IState<TContext, TStateId> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            _states[state.Id] = state;
            return this;
        }

        /// <summary>
        /// Запускає автомат: переходить у початковий стан і очікує завершення Enter.
        /// Tick не викликається до завершення Enter.
        /// </summary>
        /// <param name="initial">Ідентифікатор початкового стану.</param>
        public void Start(TStateId initial)
        {
            if (!_states.TryGetValue(initial, out var st))
                throw new InvalidOperationException($"State '{initial}' not registered.");

            Current          = st;
            CurrentId        = initial;
            _isTransitioning = true;
            _ = RunEnterAsync(st);
        }

        /// <summary>
        /// Тікає поточний стан. Ігнорується під час переходу або після Dispose.
        /// </summary>
        /// <param name="dt">Дельта-час у секундах.</param>
        public void Tick(float dt)
        {
            if (_disposed || _isTransitioning || Current == null) return;

            if (_hasForcedTransition)
            {
                ApplyForcedTransition();
                return;
            }

            if (Current.Tick(Context, dt, out var next))
                TransitionTo(next);
        }

        /// <summary>
        /// Ініціює перехід до вказаного стану, якщо автомат не у стані переходу.
        /// </summary>
        /// <param name="next">Ідентифікатор наступного стану.</param>
        public void TransitionTo(TStateId next)
        {
            if (_isTransitioning) return;
            if (EqualityComparer<TStateId>.Default.Equals(CurrentId, next)) return;
            if (!_states.ContainsKey(next))
                throw new InvalidOperationException($"State '{next}' not registered.");

            _ = RunTransitionAsync(next);
        }

        /// <summary>
        /// Примусовий перехід: якщо автомат зараз у стані переходу — перехід ставиться в чергу
        /// і застосовується одразу після завершення поточного.
        /// </summary>
        /// <param name="next">Ідентифікатор наступного стану.</param>
        public void ForceTransitionTo(TStateId next)
        {
            if (_disposed) return;
            if (EqualityComparer<TStateId>.Default.Equals(CurrentId, next)) return;

            _forcedNextId        = next;
            _hasForcedTransition = true;

            if (!_isTransitioning)
                ApplyForcedTransition();
        }

        private void ApplyForcedTransition()
        {
            if (!_hasForcedTransition) return;
            _hasForcedTransition = false;
            var next = _forcedNextId;
            if (!_states.ContainsKey(next))
                throw new InvalidOperationException($"State '{next}' not registered.");
            _ = RunTransitionAsync(next);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();
        }

        private async Task RunEnterAsync(IState<TContext, TStateId> state)
        {
            try
            {
                await state.Enter(Context, _lifetimeCts.Token);
            }
            catch (OperationCanceledException) { }
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
                await Current.Exit(Context, ct);

                if (!_states.TryGetValue(next, out var st))
                    throw new InvalidOperationException($"State '{next}' not registered.");

                Current   = st;
                CurrentId = next;

                await st.Enter(Context, ct);
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (!_disposed)
                {
                    _isTransitioning = false;
                    if (_hasForcedTransition)
                        ApplyForcedTransition();
                }
            }
        }
    }
}

