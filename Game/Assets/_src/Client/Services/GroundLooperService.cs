using System.Collections.Generic;
using Game.Client.Common;
using Game.Client.Config;
using Game.Core.Session;
using UnityEngine;

namespace Game.Client.Services
{
    /// <summary>
    /// Безкінечний тайлінг сегментів землі вздовж осі Z.
    /// При першому Tick створює пул із (SegmentsAhead + SegmentsBehind + 1) сегментів.
    /// Кожен Tick переставляє сегменти, що вийшли за хвостову зону, вперед (recycling).
    /// Дані про автомобіль беруться з <see cref="GameSession.Player"/>.
    /// </summary>
    public sealed class GroundLooperService
    {
        private readonly GroundLooperConfig _config;
        private readonly GameplaySceneRefs  _refs;
        private readonly GameSession        _session;

        private readonly Queue<Transform> _pool = new();
        private float _nextSpawnZ;
        private bool  _initialized;

        /// <summary>
        /// Створює сервіс.
        /// </summary>
        /// <param name="config">Конфігурація тайлінгу землі.</param>
        /// <param name="refs">Посилання на об'єкти сцени.</param>
        /// <param name="session">Поточна ігрова сесія.</param>
        public GroundLooperService(GroundLooperConfig config, GameplaySceneRefs refs, GameSession session)
        {
            _config  = config;
            _refs    = refs;
            _session = session;
        }

        /// <summary>
        /// Оновлює тайлінг: ініціалізує пул при першому виклику, потім переставляє сегменти.
        /// </summary>
        /// <param name="deltaTime">Дельта-час у секундах.</param>
        public void Tick(float deltaTime)
        {
            if (_config.SegmentPrefab == null) return;
            if (_session.Player       == null) return;

            if (!_initialized)
            {
                Initialize();
                return;
            }

            if (_session.IsPaused) return;

            float carZ = _session.Player.Position.z;

            while (_pool.Count > 0)
            {
                Transform back = _pool.Peek();

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

        private void Initialize()
        {
            _initialized = true;

            Transform root = _refs.GroundRoot;
            float     x    = root != null ? root.position.x : 0f;
            float     y    = root != null ? root.position.y : 0f;

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
