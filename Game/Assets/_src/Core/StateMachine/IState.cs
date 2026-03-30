using System.Threading;
using System.Threading.Tasks;

namespace Game.Core.Common.Fsm
{
    /// <summary>
    /// Стан скінченного автомата.
    /// </summary>
    /// <typeparam name="TContext">Тип контексту, що передається станам.</typeparam>
    /// <typeparam name="TStateId">Тип ідентифікатора стану.</typeparam>
    public interface IState<in TContext, TStateId>
    {
        /// <summary>Ідентифікатор стану.</summary>
        TStateId Id { get; }

        /// <summary>
        /// Викликається при вході в стан. Може бути асинхронним —
        /// наприклад, для очікування вхідної анімації.
        /// <see cref="StateMachine{TContext,TStateId}"/> не переходить до Tick, поки задача не завершиться.
        /// </summary>
        /// <param name="ctx">Контекст автомата.</param>
        /// <param name="ct">Токен скасування (скасовується при Dispose автомата).</param>
        Task Enter(TContext ctx, CancellationToken ct);

        /// <summary>
        /// Викликається при виході зі стану. Може бути асинхронним —
        /// наприклад, для очікування вихідної анімації.
        /// <see cref="StateMachine{TContext,TStateId}"/> не активує новий стан, поки задача не завершиться.
        /// </summary>
        /// <param name="ctx">Контекст автомата.</param>
        /// <param name="ct">Токен скасування.</param>
        Task Exit(TContext ctx, CancellationToken ct);

        /// <summary>
        /// Виконує оновлення стану. Повертає <c>true</c>, якщо стан запитує перехід до <paramref name="next"/>.
        /// Не викликається під час асинхронного переходу.
        /// </summary>
        /// <param name="ctx">Контекст автомата.</param>
        /// <param name="dt">Дельта-час у секундах.</param>
        /// <param name="next">Ідентифікатор наступного стану (якщо повернуто <c>true</c>).</param>
        bool Tick(TContext ctx, float dt, out TStateId next);
    }
}