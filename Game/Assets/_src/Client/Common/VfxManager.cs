using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client.Vfx
{
    /// <summary>
    /// Менеджер візуальних ефектів (VFX). Реєструє пули ефектів за рядковим ідентифікатором
    /// і дозволяє відтворювати їх у довільній позиції та орієнтації.
    /// Налаштовується через список <see cref="VfxRegistryEntry"/> у Inspector.
    /// </summary>
    public sealed class VfxManager : MonoBehaviour
    {
        [SerializeField] private Transform _poolRoot;
        [SerializeField] private List<VfxRegistryEntry> _entries = new();

        private readonly Dictionary<string, VfxPool> _pools = new(StringComparer.Ordinal);

        private void Awake()
        {
            if (_poolRoot == null)
                _poolRoot = transform;

            BuildPools();
        }

        /// <summary>
        /// Відтворює VFX-ефект за ідентифікатором у вказаній позиції та орієнтації.
        /// Якщо ідентифікатор не зареєстровано — виводить попередження у консоль.
        /// </summary>
        /// <param name="id">Ідентифікатор ефекту.</param>
        /// <param name="position">Позиція відтворення у світовому просторі.</param>
        /// <param name="rotation">Орієнтація ефекту.</param>
        public void Play(string id, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(id, out var pool))
            {
                Debug.LogWarning($"VFX with id '{id}' is not registered.");
                return;
            }

            var instance = pool.Get();
            instance.Play(position, rotation, pool.Release);
        }

        private void BuildPools()
        {
            _pools.Clear();

            for (var i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                if (entry == null || entry.Prefab == null)
                    continue;

                var id = entry.Id;
                if (string.IsNullOrWhiteSpace(id))
                {
                    Debug.LogWarning($"VFX entry at index {i} has empty id.");
                    continue;
                }

                if (_pools.ContainsKey(id))
                {
                    Debug.LogWarning($"VFX id '{id}' is already registered.");
                    continue;
                }

                var poolParent = new GameObject($"{id}_Pool").transform;
                poolParent.SetParent(_poolRoot, false);

                var pool = new VfxPool(entry.Prefab, poolParent);
                _pools.Add(id, pool);

                for (var j = 0; j < entry.PrewarmCount; j++)
                    pool.Prewarm();
            }
        }

        /// <summary>Пул екземплярів одного VFX-префабу.</summary>
        private sealed class VfxPool
        {
            private readonly PooledVfx _prefab;
            private readonly Transform _parent;
            private readonly Stack<PooledVfx> _items = new();

            public VfxPool(PooledVfx prefab, Transform parent)
            {
                _prefab = prefab;
                _parent = parent;
            }

            public void Prewarm()
            {
                var instance = Create();
                Release(instance);
            }

            public PooledVfx Get()
            {
                return _items.Count > 0 
                    ? _items.Pop() 
                    : Create();
            }

            public void Release(PooledVfx instance)
            {
                if (instance == null)
                    return;

                instance.transform.SetParent(_parent, false);
                _items.Push(instance);
            }

            private PooledVfx Create()
            {
                var instance = UnityEngine.Object.Instantiate(_prefab, _parent);
                instance.gameObject.SetActive(false);
                return instance;
            }
        }

        /// <summary>
        /// Запис реєстру VFX для налаштування у Inspector.
        /// Зв'язує рядковий ідентифікатор із префабом та кількістю прогріву.
        /// </summary>
        [Serializable]
        public sealed class VfxRegistryEntry
        {
            [SerializeField] private string _id;
            [SerializeField] private PooledVfx _prefab;
            [SerializeField] private int _prewarmCount = 0;

            public string Id => string.IsNullOrWhiteSpace(_id) && _prefab != null
                ? _prefab.name
                : _id;

            public PooledVfx Prefab => _prefab;
            public int PrewarmCount => _prewarmCount;
        }
    }
}