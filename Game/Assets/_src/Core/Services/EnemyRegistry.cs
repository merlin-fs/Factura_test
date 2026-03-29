using Game.Core.Events;

namespace Game.Core.Services
{
    /// <summary>
    /// Tracks kill count and raises EnemyKilled events.
    /// Does NOT manage pooling — that belongs to EnemyPool.
    /// </summary>
    public sealed class EnemyRegistry
    {
        private readonly GameEvents _events;
        private int _killCount;

        public EnemyRegistry(GameEvents events)
        {
            _events = events;
        }

        /// <summary>Call when an enemy is confirmed dead (hp &lt;= 0).</summary>
        public void ReportKill()
        {
            _events.EnemyKilled.Invoke(++_killCount);
        }
    }
}
