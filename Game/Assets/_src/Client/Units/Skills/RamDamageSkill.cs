using System;
using System.Collections.Generic;
using Game.Core.Common;
using Game.Core.Services;
using Game.Core.Units;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Client.Skills
{
    [Serializable]
    public sealed class RamDamageSkill : ISkill, IHitHandler, ITickSystem
    {
        [SerializeField, Min(0f)] private float radius = 1.5f;
        [SerializeField, Min(0)]  private int   damage = 10;
        [SerializeField, Min(0f)] private float length = 3f;
        [SerializeField, Min(0f)] private float cooldown = 0.5f;
        
        private DamageService _damageService;
        private HitService _hitService;
        private TickSystemRegistry _tickRegistry;

        private readonly Dictionary<Unit, float> _cooldowns = new();

        [Inject]
        private void Inject(DamageService damageService, HitService hitService, TickSystemRegistry tickRegistry)
        {
            _damageService = damageService;
            _hitService = hitService;
            _tickRegistry = tickRegistry;
        }
        public void Initialize()
        {
            _tickRegistry.Register(this);
        }

        public void Dispose()
        {
            _tickRegistry.Unregister(this);
        }
        
        public void Tick(float dt)
        {
            /*
            var query = new HitQuery(
                HitQueryType.OverlapCapsule,
                source,
                origin,
                direction,
                0f,
                radius,
                _hitMask,
                8,
                origin,
                origin + direction.normalized *length);
            _hitService.Process(query, this);
            */
        }
        
        public bool CanApply(Unit source, Unit target)
        {
            if (source == null || target == null)
                return false;

            return !_cooldowns.TryGetValue(target, out var nextTime) || !(Time.time < nextTime);
        }

        public void Apply(Unit source, Unit target)
        {
            if (!CanApply(source, target))
                return;

            _damageService.Apply(new DamageRequest(source, target, damage));
            _cooldowns[target] = Time.time + cooldown;
        }

        public void Handle(Unit source, Unit target)
        {
            Apply(source, target);
        }

        public ISkill Clone() => new RamDamageSkill{ radius = radius, damage = damage, length = length, cooldown = cooldown };
    }
}