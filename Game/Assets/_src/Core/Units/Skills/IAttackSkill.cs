using UnityEngine;

namespace Game.Core.Units
{
    /// <summary>
    /// Навичка атаки юніта.
    /// </summary>
    public interface IAttackSkill : ISkill
    {
        /// <summary>
        /// Перевіряє, чи може навичка бути використана в поточному контексті.
        /// </summary>
        /// <param name="context">Контекст атаки.</param>
        bool CanUse(in AttackContext context);

        /// <summary>
        /// Застосовує навичку атаки.
        /// </summary>
        /// <param name="context">Контекст атаки.</param>
        void Use(in AttackContext context);
    }

    /// <summary>
    /// Дані контексту для виконання атаки.
    /// </summary>
    /// <param name="Source">Атакуючий юніт.</param>
    /// <param name="Origin">Початкова точка атаки у світовому просторі.</param>
    /// <param name="Direction">Напрямок атаки.</param>
    /// <param name="HitMask">Маска шарів для визначення цілей.</param>
    /// <param name="Target">Конкретна ціль (може бути null для атак по площі).</param>
    public record AttackContext(Unit Source, Vector3 Origin, Vector3 Direction, LayerMask HitMask, Unit Target);
}
