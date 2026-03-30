namespace Game.Core
{
    /// <summary>
    /// Стан ворожого юніта у скінченному автоматі.
    /// </summary>
    public enum EnemyState
    {
        /// <summary>Простій — ворог нерухомо стоїть на місці.</summary>
        Idle,
        /// <summary>Блукання — ворог рухається у випадковому напрямку.</summary>
        Wander,
        /// <summary>Переслідування — ворог рухається до гравця.</summary>
        Chase,
        /// <summary>Атака — ворог атакує гравця.</summary>
        Attack,
        /// <summary>Смерть — загальна анімація загибелі.</summary>
        Dead,
        /// <summary>Смерть від снаряду.</summary>
        DeadByProjectile,
        /// <summary>Смерть від наїзду автомобіля.</summary>
        DeadByRam
    }
}
