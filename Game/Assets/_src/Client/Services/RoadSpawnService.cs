using System.Linq;
using Game.Core.Common;
using Game.Core.Session;
using Game.Client.Config;
using Game.Client.Views;
using Game.Core.Units;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Client.Services
{
    public sealed class RoadSpawnService
    {
        private readonly LevelConfig _levelConfig;
        private GameSession _session;
        private IEnemyFactory _enemyFactory;
        private PrefabPool<EnemyView> _enemyPool;

        private float _nextSpawnAtCarZ;
        private bool  _initialized;

        public RoadSpawnService(LevelConfig levelConfig)
        {
            _levelConfig  = levelConfig;
        }

        [Inject]
        private void Inject(GameSession session, IEnemyFactory enemyFactory,
            PrefabPool<EnemyView> enemyPool)
        {
            _session      = session;
            _enemyFactory = enemyFactory;
            _enemyPool    = enemyPool;
        }

        public void Tick(float deltaTime)
        {
            if (_session.IsPaused) return;

            var spawnConfig = _levelConfig.RoadSpawn;
            if (spawnConfig == null) return;
            if (spawnConfig.Patterns == null || spawnConfig.Patterns.Length == 0) return;

            var carZ = _session.Player.Position.z;

            if (!_initialized)
            {
                _initialized        = true;
                _nextSpawnAtCarZ    = _session.StartPositionZ + spawnConfig.InitialSpawnDistance;
            }

            if (carZ < _nextSpawnAtCarZ) return;
            if (_enemyPool.ActiveCount >= spawnConfig.MaxAliveEnemies) return;

            var pattern = PickPattern(spawnConfig);
            if (pattern == null || pattern.Entries.Length == 0)
            {
                ScheduleNext(carZ, spawnConfig);
                return;
            }

            var finishZ    = _session.StartPositionZ + _levelConfig.LevelLength;
            var groupBaseZ = Mathf.Min(
                carZ + Random.Range(spawnConfig.MinSpawnAheadDistance, spawnConfig.MaxSpawnAheadDistance),
                finishZ);

            SpawnPattern(pattern, groupBaseZ, spawnConfig);
            ScheduleNext(carZ, spawnConfig);
        }

        // ------------------------------------------------------------------ helpers

        private void SpawnPattern(RoadSpawnPatternData pattern, float groupBaseZ, RoadSpawnConfig cfg)
        {
            var lanes = cfg.LaneOffsetsX;
            if (lanes == null || lanes.Length == 0) return;

            foreach (RoadSpawnEntryData entry in pattern.Entries)
            {
                if (entry.EnemyConfig == null || entry.EnemyConfig.Prefab == null) continue;

                var laneIndex = entry.UseRandomLane
                    ? Random.Range(0, lanes.Length)
                    : Mathf.Clamp(entry.LaneIndex, 0, lanes.Length - 1);

                var spawnX = lanes[laneIndex]
                             + entry.LateralOffset
                             + Random.Range(-entry.LateralJitter, entry.LateralJitter);

                var spawnZ = groupBaseZ
                             + entry.ForwardOffset
                             + Random.Range(-entry.ForwardJitter, entry.ForwardJitter);

                _enemyFactory.Spawn(entry.EnemyConfig,
                    new Vector3(spawnX, cfg.SpawnY, spawnZ),
                    Quaternion.identity);
            }
        }

        private void ScheduleNext(float carZ, RoadSpawnConfig cfg)
        {
            _nextSpawnAtCarZ = carZ + Random.Range(
                cfg.MinDistanceBetweenGroups,
                cfg.MaxDistanceBetweenGroups);
        }

        private static RoadSpawnPatternData PickPattern(RoadSpawnConfig cfg)
        {
            var total = cfg.Patterns.Where(p => p is { Weight: > 0 }).Sum(p => p.Weight);

            if (total <= 0) return null;

            var roll = Random.Range(0, total);
            foreach (var p in cfg.Patterns)
            {
                if (p is not { Weight: > 0 }) continue;
                if (roll < p.Weight) return p;
                roll -= p.Weight;
            }
            return null;
        }
    }
}
