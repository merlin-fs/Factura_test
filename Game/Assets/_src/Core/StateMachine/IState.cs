using System.Threading;
using System.Threading.Tasks;

namespace Game.Core.Common.Fsm
{
    /// <summary>
    /// Стан автомата.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TStateId"></typeparam>
    public interface IState<in TContext, TStateId>
    {
        TStateId Id { get; }

        /// <summary>
        /// Викликається при вході в стан.
        /// Може бути асинхронним — наприклад, очікувати вхідну анімацію.
        /// StateMachine не переходить до Tick поки задача не завершиться.
        /// </summary>
        Task Enter(TContext ctx, CancellationToken ct);

        /// <summary>
        /// Викликається при виході зі стану.
        /// Може бути асинхронним — наприклад, очікувати вихідну анімацію.
        /// StateMachine не активує новий стан поки задача не завершиться.
        /// </summary>
        Task Exit(TContext ctx, CancellationToken ct);

        /// <summary>
        /// Повертає true, якщо стан запитує перехід до наступного стану.
        /// Не викликається під час async-переходу.
        /// </summary>
        bool Tick(TContext ctx, float dt, out TStateId next);
    }
}