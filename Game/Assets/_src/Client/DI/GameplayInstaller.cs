using Game.Core.Session;
using Game.Client.Bootstrap;
using Game.Client.Input;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Game.Client.DI
{
    /// <summary>
    /// Root-level (scene-wide) bindings only:
    /// scene references, input wrappers, session infrastructure.
    /// Everything gameplay-session-scoped lives in GameSessionBuilder.
    /// </summary>
    public sealed class GameplayInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private GameplaySceneRefs sceneRefs;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(sceneRefs);

            // Input wrappers — одна пара на всё время жизни сцены
            builder.Register<IHorizontalDragInput>(
                _ => new HorizontalDragInput(sceneRefs.HorizontalDragAction),
                Lifetime.Singleton);
            builder.Register<IFireInput>(
                _ => new FireInput(sceneRefs.FireAction),
                Lifetime.Singleton);

            // Session infrastructure
            builder.Register<SessionCoordinator>(Lifetime.Singleton);
            builder.Register<ISessionBuilder, GameSessionBuilder>(Lifetime.Singleton);
        }
    }
}