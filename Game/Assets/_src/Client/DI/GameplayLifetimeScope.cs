using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Client.DI
{
    /// <summary>
    /// Корневой контейнер сцены Gameplay.
    /// Добавьте этот компонент на GameObject в сцене вместо базового LifetimeScope.
    /// Назначьте ссылку на GameplayInstaller (MonoBehaviour на той же или другой GameObject).
    /// В поле AutoInjectGameObjects добавьте SessionEntryPoint, чтобы он получил инжекцию.
    /// </summary>
    public sealed class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameplayInstaller installer;

        protected override void Configure(IContainerBuilder builder)
        {
            installer.Install(builder);
        }
    }
}

