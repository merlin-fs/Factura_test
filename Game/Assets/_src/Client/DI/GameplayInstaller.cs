using Game.Core.Session;
using Game.Client.Bootstrap;
using Game.Client.Input;
using Game.Core.Common;
using Game.Core.Events;
using Game.Core.Services;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Game.Client.DI
{
    public sealed class GameplayInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private GameplaySceneRefs sceneRefs;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(sceneRefs);

            builder.RegisterFactory<IHorizontalDragInput>(c => new HorizontalDragInput(sceneRefs.HorizontalDragAction), Lifetime.Singleton, Resolution.Lazy);
            builder.RegisterFactory<IFireInput>(c => new FireInput(sceneRefs.FireAction), Lifetime.Singleton, Resolution.Lazy);

            builder.RegisterType<SessionCoordinator>(Lifetime.Singleton);
            builder.RegisterType<ISessionBuilder, GameSessionBuilder>(Lifetime.Singleton);
            
            builder.RegisterType<IUnitRegistry, UnitRegistry>(Lifetime.Singleton);
            builder.RegisterType<ITargetsProvider, TargetsProvider>(Lifetime.Singleton);
            builder.RegisterType<HitService>(Lifetime.Singleton);

            builder.RegisterType<DamageService>(Lifetime.Singleton);
            builder.RegisterType<TickSystemRegistry>(Lifetime.Singleton);
            builder.RegisterType<GameEvents>(Lifetime.Singleton);
        }
    }
}