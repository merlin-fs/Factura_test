using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Common
{
    /// <summary>
    /// Generic prefab-based object pool with active-instance tracking.
    /// Supports multiple different prefabs of the same type simultaneously.
    /// Call Dispose() at session end to destroy all pooled GameObjects.
    /// </summary>
    public sealed class PrefabPool<TView> : IDisposable where TView : MonoBehaviour
    {
        // Per-prefab stack of free instances
        private readonly Dictionary<TView, Stack<TView>> _free     = new();
        // Maps each instance back to the prefab it was created from
        private readonly Dictionary<TView, TView>        _prefabOf = new();
        private readonly List<TView>                     _active   = new();
        private readonly FactoryMethod _factoryMethod;

        public IReadOnlyList<TView> Active     => _active;
        public int                  ActiveCount => _active.Count;

        public delegate TView FactoryMethod(TView prefab, Vector3 position, Quaternion rotation);

        public PrefabPool(FactoryMethod factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }
        
        
        /// <summary>Take an instance from the pool (or instantiate a new one).</summary>
        public TView Get(TView prefab, Vector3 position, Quaternion rotation)
        {
            if (!_free.TryGetValue(prefab, out var stack))
            {
                stack = new Stack<TView>();
                _free[prefab] = stack;
            }

            TView view;
            if (stack.Count > 0)
            {
                view = stack.Pop();
                view.transform.SetPositionAndRotation(position, rotation);
                view.gameObject.SetActive(true);
            }
            else
            {
                view = _factoryMethod(prefab, position, rotation);
                _prefabOf[view] = prefab;
            }

            _active.Add(view);
            return view;
        }

        /// <summary>Return an instance to the pool (deactivates its GameObject).</summary>
        public void Return(TView view)
        {
            if (view == null) return;

            _active.Remove(view);
            view.gameObject.SetActive(false);

            if (_prefabOf.TryGetValue(view, out var prefab) && _free.TryGetValue(prefab, out var stack))
                stack.Push(view);
            else
                UnityEngine.Object.Destroy(view.gameObject); // fallback for leaked instances
        }

        /// <summary>Destroy all pooled GameObjects.</summary>
        public void Dispose()
        {
            _active.Clear();

            foreach (var stack in _free.Values)
                foreach (var view in stack)
                    if (view != null)
                        UnityEngine.Object.Destroy(view.gameObject);

            _free.Clear();
            _prefabOf.Clear();
        }
    }
}
