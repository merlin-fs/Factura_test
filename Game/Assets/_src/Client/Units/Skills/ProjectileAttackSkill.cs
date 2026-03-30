using System;
using Game.Client.Config;
using Game.Core.Units;
using VContainer;
using UnityEngine;

namespace Game.Client.Units
{
    /// <summary>
    /// Навичка атаки снарядом. Використовує <see cref="ProjectileFactory"/> для спауну
    /// снаряду у точці дула у вказаному напрямку.
    /// </summary>
    [Serializable]
    public class ProjectileAttackSkill : IAttackSkill
    {
        [SerializeField] private ProjectileConfig projectileConfig;

        [NonSerialized] private ProjectileFactory _projectileFactory;

        [Inject]
        private void Inject(ProjectileFactory projectileFactory) => _projectileFactory = projectileFactory;

        /// <inheritdoc/>
        public void Dispose() { }

        /// <inheritdoc/>
        public ISkill Clone() => new ProjectileAttackSkill { projectileConfig = projectileConfig };

        /// <inheritdoc/>
        public bool CanUse(in AttackContext context)
            => context.Source != null && context.Direction.sqrMagnitude > 0.0001f;

        /// <inheritdoc/>
        public void Use(in AttackContext context)
        {
            if (!CanUse(context)) return;
            var projectile = _projectileFactory.Spawn(
                projectileConfig, context.HitMask, context.Origin, context.Direction);
            projectile.Activate(context.Source, context.Target, context.Direction);
        }
    }
}
