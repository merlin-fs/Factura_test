using System;
using Game.Core.Units;

namespace Game.Core.Services
{
    /// <summary>
    /// Central damage manager. All damage must flow through here —
    /// single point for future modifiers (armour, invincibility, logging, etc.).
    /// </summary>
    public sealed class DamageService
    {
        public event Action<Unit, int> DamageApplied;
        public event Action<Unit>      Died;

        public void Apply(DamageRequest request)
        {
            if (request.Target == null) return;
            if (!request.Target.Stats.Has<HpStat>()) return;

            int finalDamage = request.Amount;

            // позже здесь можно учесть armor, shield, buffs и т.д.

            var hp = request.Target.Stats.Get<HpStat>();
            hp.Apply(-finalDamage);
            
            DamageApplied?.Invoke(request.Target, finalDamage);

            if (!hp.IsAlive)
                Died?.Invoke(request.Target);
        }
    }
}
