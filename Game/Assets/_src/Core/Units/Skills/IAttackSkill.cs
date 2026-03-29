using UnityEngine;

namespace Game.Core.Units
{
    public interface IAttackSkill : ISkill
    {
        bool CanUse(in AttackContext context);
        void Use(in AttackContext context);
    }

    public record AttackContext(Unit Source, Vector3 Origin, Vector3 Direction, LayerMask HitMask, Unit Target);
}
