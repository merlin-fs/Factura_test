using System;
using Game.Core.Services;
using Game.Core.Units;
using VContainer;
using UnityEngine;

namespace Game.Client.Units
{
    /// <summary>
    /// Навичка ближнього бою. Перевіряє наявність цілі у заданому радіусі та завдає шкоди
    /// через <see cref="DamageService"/> при успішному влучанні.
    /// </summary>
    [Serializable]
    public class MeleeAttackSkill : IAttackSkill, IHitHandler
    {
        [SerializeField, Min(0f)] private float radius = 1.5f;
        [SerializeField, Min(0)]  private int   damage = 10;

        [NonSerialized] private DamageService _damageService;
        [NonSerialized] private HitService    _hitService;

        [Inject]
        private void Inject(DamageService damageService, HitService hitService)
        {
            _damageService = damageService;
            _hitService    = hitService;
        }

        /// <inheritdoc/>
        public void Dispose() { }

        /// <inheritdoc/>
        public ISkill Clone() => new MeleeAttackSkill { radius = radius, damage = damage };

        /// <inheritdoc/>
        public bool CanUse(in AttackContext context)
        {
            if (context.Source == null || context.Target == null) return false;
            return Vector3.Distance(context.Source.Position, context.Target.Position) <= radius;
        }

        /// <inheritdoc/>
        public void Use(in AttackContext context)
        {
            if (!CanUse(context)) return;
            _hitService.ProcessFirst(new HitQuery(
                HitQueryType.OverlapSphere,
                context.Source,
                context.Origin + context.Direction * (radius * 0.5f),
                context.Direction,
                0f, radius, context.HitMask, 1), this);
        }

        /// <inheritdoc/>
        public void Handle(Unit source, Unit target)
            => _damageService.Apply(new DamageRequest(source, target, damage));
    }
}
