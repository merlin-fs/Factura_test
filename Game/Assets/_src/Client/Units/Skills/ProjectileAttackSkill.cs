using System;
using Game.Client.Config;
using Game.Core.Units;
using VContainer;
using UnityEngine;

namespace Game.Client.Units
{
    [Serializable]
    public class ProjectileAttackSkill : IAttackSkill
    {
        [SerializeField] private ProjectileConfig projectileConfig;

        [NonSerialized] private ProjectileFactory _projectileFactory;

        [Inject]
        private void Inject(ProjectileFactory projectileFactory)
            => _projectileFactory = projectileFactory;

        public void Dispose() { }

        public ISkill Clone() => new ProjectileAttackSkill { projectileConfig = projectileConfig };

        public bool CanUse(in AttackContext context)
            => context.Source != null && context.Direction.sqrMagnitude > 0.0001f;

        public void Use(in AttackContext context)
        {
            if (!CanUse(context)) return;
            var projectile = _projectileFactory.Spawn(
                projectileConfig, context.HitMask, context.Origin, context.Direction);
            projectile.Activate(context.Source, context.Target, context.Direction);
        }
    }
}
