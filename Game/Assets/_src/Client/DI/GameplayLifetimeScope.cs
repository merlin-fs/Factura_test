using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Client.DI
{
    /// <summary>
    /// Кореневий DI-контейнер сцени Gameplay.
    /// Додайте цей компонент на GameObject у сцені замість базового <c>LifetimeScope</c>.
    /// Призначте посилання на <see cref="GameplayInstaller"/> та додайте <c>SessionEntryPoint</c>
    /// до поля <c>AutoInjectGameObjects</c>, щоб він отримав впровадження залежностей.
    /// </summary>
    public sealed class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameplayInstaller installer;

        /// <inheritdoc/>
        protected override void Configure(IContainerBuilder builder)
        {
            installer.Install(builder);
        }
    }
}
