using Game.Core.Events;

namespace Game.Core.Services
{
    /// <summary>
    /// Tracks live enemy count and kill count; raises EnemyKilled events.
    /// Live count is the authoritative source — not the pool's ActiveCount.
    /// </summary>
    public sealed class EnemyRegistry
    {
        private readonly GameEvents _events;
        private int _killCount;

        /// <summary>Number of enemies currently alive in the session.</summary>
        public int LiveCount { get; private set; }

        public EnemyRegistry(GameEvents events)
        {
            _events = events;
        }

        /// <summary>Call when an enemy is spawned.</summary>
        public void TrackSpawn() => LiveCount++;

        /// <summary>Call when an enemy is confirmed dead (hp &lt;= 0).</summary>
        public void ReportKill()
        {
            LiveCount--;
            _events.EnemyKilled.Invoke(++_killCount);
        }

        /// <summary>Call when an enemy leaves the play area without dying (culled).</summary>
        public void TrackRelease() => LiveCount--;
    }
}
