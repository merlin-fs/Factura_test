using FTg.Common.Observables;
using Game.Core.Session;
using UnityEngine;
using VContainer;

namespace Game.Client.UI
{
    /// <summary>
    /// Базовий клас для вікон UI, що прив'язані до ігрової сесії.
    /// Автоматично підписується на події <see cref="SessionCoordinator.NewSession"/>
    /// та <see cref="SessionCoordinator.EndSession"/> і викликає <see cref="Bind"/>/<see cref="Unbind"/>.
    /// </summary>
    public abstract class BaseGameWindow : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        /// <summary>
        /// Впроваджує залежності та підписується на події сесій.
        /// </summary>
        [Inject]
        public void Construct(SessionCoordinator coordinator)
        {
            coordinator.NewSession.Subscribe(Bind).AddTo(_disposables);
            coordinator.EndSession.Subscribe(Unbind).AddTo(_disposables);
        }

        /// <summary>
        /// Викликається при старті нової сесії. Перевизначте для підписки на дані сесії.
        /// </summary>
        protected virtual void Bind(GameSession session) { }

        /// <summary>
        /// Викликається при завершенні сесії. Перевизначте для очищення підписок.
        /// </summary>
        protected virtual void Unbind(GameSession session) { }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}