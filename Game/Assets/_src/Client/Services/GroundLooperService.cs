using System.Collections.Generic;
using Game.Client.Bootstrap;
using Game.Client.Config;
using Game.Core.Session;
using UnityEngine;

namespace Game.Client.Services
{
    /// <summary>
    /// Бесконечный тайлинг сегментов земли вдоль оси Z.
    ///
    /// Принцип работы:
    ///   При первом Tick создаётся пул из (SegmentsAhead + SegmentsBehind + 1) сегментов.
    ///   Каждый Tick: сегменты, ушедшие за хвостовую зону, переставляются вперёд (recycling).
    ///
    /// Данные о машине берутся из GameSession.Player.Position.
    /// Параметры и префаб задаются через GroundLooperConfig.
    /// Сегменты создаются под GroundRoot из GameplaySceneRefs.
    /// </summary>
    public sealed class GroundLooperService
    {
        private readonly GroundLooperConfig _config;
        private readonly GameplaySceneRefs  _refs;
        private readonly GameSession        _session;

        private readonly Queue<Transform> _pool = new();
        private float _nextSpawnZ;
        private bool  _initialized;

        // ─────────────────────────────────────────────────────────────────

        public GroundLooperService(GroundLooperConfig config, GameplaySceneRefs refs, GameSession session)
        {
            _config  = config;
            _refs    = refs;
            _session = session;
        }

        // ─────────────────────────────────────────────────────────────────

        public void Tick(float deltaTime)
        {
            if (_config.SegmentPrefab == null) return;
            if (_session.Player       == null) return;

            // Инициализация — первый кадр, когда Player уже создан
            if (!_initialized)
            {
                Initialize();
                return;
            }

            if (_session.IsPaused) return;

            float carZ = _session.Player.Position.z;

            // Переставляем сегменты, вышедшие за хвостовую зону, вперёд
            while (_pool.Count > 0)
            {
                Transform back = _pool.Peek();

                // Задний конец сегмента вышел за пределы удерживаемой зоны позади?
                if (back.position.z + _config.SegmentLength < carZ - _config.SegmentsBehind * _config.SegmentLength)
                {
                    _pool.Dequeue();
                    back.position = new Vector3(back.position.x, back.position.y, _nextSpawnZ);
                    _pool.Enqueue(back);
                    _nextSpawnZ += _config.SegmentLength;
                }
                else
                {
                    break;
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────

        private void Initialize()
        {
            _initialized = true;

            Transform root = _refs.GroundRoot;
            float     x    = root != null ? root.position.x : 0f;
            float     y    = root != null ? root.position.y : 0f;

            // Пул стартует чуть позади машины
            _nextSpawnZ = _session.Player.Position.z - _config.SegmentsBehind * _config.SegmentLength;

            int total = _config.SegmentsAhead + _config.SegmentsBehind + 1;
            for (int i = 0; i < total; i++)
            {
                var pos = new Vector3(x, y, _nextSpawnZ);
                var obj = Object.Instantiate(_config.SegmentPrefab, pos, Quaternion.identity, root);
                _pool.Enqueue(obj.transform);
                _nextSpawnZ += _config.SegmentLength;
            }
        }
    }
}

