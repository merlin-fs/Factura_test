using System;
using System.Collections.Generic;
using Game.Core.Common;
using Game.Core.Services;
using Game.Core.Units;
using VContainer;
using UnityEngine;

namespace Game.Client.Skills
{
    /// <summary>
    /// Навичка наїзду автомобілем. Щокадру перевіряє зіткнення з капсулою перед машиною
    /// та завдає шкоди кожному ворогу у зоні з урахуванням кулдауну на ціль.
    /// </summary>
    [Serializable]
    public sealed class RamDamageSkill : ISkill, IHitHandler, ITickSystem
    {
        [SerializeField, Min(0f)] private float radius  = 1.5f;
        [SerializeField, Min(0)]  private int   damage  = 10;
        [SerializeField, Min(0f)] private float length  = 3f;
        [SerializeField, Min(0f)] private float cooldown = 0.5f;

        private DamageService      _damageService;
        private HitService         _hitService;
        private TickSystemRegistry _tickRegistry;
        private Unit               _owner;

        private readonly Dictionary<Unit, float> _cooldowns = new();

        [Inject]
        private void Inject(DamageService damageService, HitService hitService, TickSystemRegistry tickRegistry)
        {
            _damageService = damageService;
            _hitService    = hitService;
            _tickRegistry  = tickRegistry;
            _tickRegistry.Register(this);
        }

        /// <inheritdoc/>
        public void Bind(Unit owner) => _owner = owner;

        /// <inheritdoc/>
        public ISkill Clone() => new RamDamageSkill { radius = radius, damage = damage, length = length, cooldown = cooldown };

        /// <inheritdoc/>
        public void Dispose() => _tickRegistry?.Unregister(this);

        public void Tick(float dt)
        {
            if (_owner == null) return;

            var origin    = _owner.Position;
            var forward   = _owner.Forward;
            var point2    = origin + forward * length;

            var query = new HitQuery(
                HitQueryType.OverlapCapsule,
                _owner,
                origin,
                forward,
                0f,
                radius,
                _owner.TargetMask,
                8,
                origin,
                point2);

            _hitService.Process(query, this);
        }
        
        /// <summary>
        /// Перевіряє, чи не закінчився кулдаун для цілі.
        /// </summary>
        public bool CanApply(Unit source, Unit target)
        {
            if (source == null || target == null)
                return false;

            return !_cooldowns.TryGetValue(target, out var nextTime) || !(Time.time < nextTime);
        }

        /// <summary>
        /// Застосовує шкоду до цілі та встановлює кулдаун.
        /// </summary>
        public void Apply(Unit source, Unit target)
        {
            if (!CanApply(source, target))
                return;

            _damageService.Apply(new DamageRequest(source, target, damage, DamageSource.Ram));
            _cooldowns[target] = Time.time + cooldown;
        }

        /// <inheritdoc/>
        public void Handle(Unit source, Unit target)
        {
            Apply(source, target);
        }
    }
}