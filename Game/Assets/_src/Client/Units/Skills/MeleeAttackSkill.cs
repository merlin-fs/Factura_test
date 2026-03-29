using System;
using Game.Core.Services;
using Game.Core.Units;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Client.Units
{
    [Serializable]
    public class MeleeAttackSkill : IAttackSkill, IHitHandler
    {
        [SerializeField, Min(0f)] private float radius = 1.5f;
        [SerializeField, Min(0)]  private int   damage = 10;

        [NonSerialized] private DamageService _damageService;
        [NonSerialized] private HitService _hitService;

        [Inject]
        private void Inject(DamageService damageService, HitService hitService)
        {
            _damageService = damageService;
            _hitService = hitService;
        }

        public void Initialize(){}
        public void Dispose(){}

        public bool CanUse(in AttackContext context)
        {
            if (context.Source == null || context.Target == null)
                return false;

            var distance = Vector3.Distance(context.Source.Position, context.Target.Position);
            return distance <= radius;
        }

        public void Use(in AttackContext context)
        {
            if (!CanUse(context))
                return;

            var query = new HitQuery(
                HitQueryType.OverlapSphere,
                context.Source,
                context.Origin + context.Direction * (radius * 0.5f),
                context.Direction,
                0f,
                radius,
                context.HitMask,
                1);

            _hitService.ProcessFirst(query, this);
        }

        public void Handle(Unit source, Unit target)
        {
            _damageService.Apply(new DamageRequest(source, target, damage));
        }
        public ISkill Clone() => new MeleeAttackSkill { radius = radius, damage = damage };
    }
}
