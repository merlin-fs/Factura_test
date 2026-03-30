namespace Game.Core.Services
{
    /// <summary>
    /// Визначає джерело (тип) шкоди, що спричинила влучання або загибель.
    /// Використовується для вибору анімації смерті юніта.
    /// </summary>
    public enum DamageSource
    {
        /// <summary>Невідоме джерело.</summary>
        Unknown,
        /// <summary>Шкода від снаряду.</summary>
        Projectile,
        /// <summary>Шкода від наїзду автомобіля.</summary>
        Ram,
        /// <summary>Шкода від ближнього бою.</summary>
        Melee,
    }
}
