using Game.Core.Session;
using Game.Client.Common;
using Game.Client.Input;
using Game.Client.Session;
using Game.Client.UI;
using Game.Client.Vfx;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Game.Client.DI
{
    /// <summary>
    /// Кореневий (сцено-рівневий) інсталлер DI-контейнера сцени Gameplay.
    /// Реєструє посилання на сцену, обгортки вводу та інфраструктуру сесії.
    /// Все, що має область видимості ігрової сесії, живе в <c>GameSessionBuilder</c>.
    /// </summary>
    public sealed class GameplayInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private GameplaySceneRefs sceneRefs;
        [SerializeField] private VfxManager vfxManager;

        /// <inheritdoc/>
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(sceneRefs);
            builder.RegisterInstance(vfxManager);

            builder.Register<IHorizontalDragInput>(
                _ => new HorizontalDragInput(sceneRefs.HorizontalDragAction),
                Lifetime.Singleton);
            builder.Register<IFireInput>(
                _ => new FireInput(sceneRefs.FireAction),
                Lifetime.Singleton);

            builder.Register<SessionCoordinator>(Lifetime.Singleton);
            builder.Register<ISessionBuilder, GameSessionBuilder>(Lifetime.Singleton);
            builder.RegisterEntryPoint<UiSystem>()
                .WithParameter("hudGame", sceneRefs.HudGame)
                .WithParameter("beginGamePanel", sceneRefs.BeginGamePanel)
                .WithParameter("winPanel", sceneRefs.WinPanel)
                .WithParameter("losePanel", sceneRefs.LosePanel);
        }
    }
}