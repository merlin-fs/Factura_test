using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Core.Common
{
    /// <summary>
    /// Узагальнений пул об'єктів на основі префабів із відстеженням активних екземплярів.
    /// Підтримує кілька різних префабів одного типу одночасно.
    /// Викличте <see cref="Dispose"/> наприкінці сесії, щоб знищити всі GameObject-и пулу.
    /// </summary>
    public sealed class PrefabPool<TView> : IDisposable where TView : MonoBehaviour
    {
        private readonly Dictionary<TView, Stack<TView>> _free     = new();
        private readonly Dictionary<TView, TView>        _prefabOf = new();
        private readonly List<TView>                     _active   = new();
        private readonly Func<TView, Transform, TView> _factoryMethod;
        private readonly Transform _parent;

        /// <summary>Список активних (виданих із пулу) екземплярів.</summary>
        public IReadOnlyList<TView> Active     => _active;
        /// <summary>Кількість активних екземплярів.</summary>
        public int                  ActiveCount => _active.Count;

        /// <summary>
        /// Створює новий пул.
        /// </summary>
        /// <param name="factoryMethod">Фабричний метод для інстанціювання нових об'єктів.</param>
        /// <param name="parent">Transform-батько для всіх екземплярів пулу.</param>
        public PrefabPool(Func<TView, Transform, TView> factoryMethod, Transform parent)
        {
            _factoryMethod = factoryMethod;
            _parent        = parent;
        }

        /// <summary>
        /// Повертає екземпляр із пулу або створює новий, якщо вільних немає.
        /// </summary>
        /// <param name="prefab">Префаб, що є джерелом екземпляра.</param>
        /// <param name="position">Початкова позиція.</param>
        /// <param name="rotation">Початковий поворот.</param>
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
                view = _factoryMethod(prefab, _parent);
                view.name = $"{prefab.name}_{_active.Count}";
                view.transform.SetPositionAndRotation(position, rotation);
                _prefabOf[view] = prefab;
            }

            _active.Add(view);
            return view;
        }

        /// <summary>
        /// Повертає екземпляр до пулу та деактивує його GameObject.
        /// </summary>
        /// <param name="view">Екземпляр для повернення.</param>
        public void Return(TView view)
        {
            if (view == null) return;

            _active.Remove(view);
            view.gameObject.SetActive(false);

            if (_prefabOf.TryGetValue(view, out var prefab) && _free.TryGetValue(prefab, out var stack))
                stack.Push(view);
            else
                UnityEngine.Object.Destroy(view.gameObject);
        }

        /// <summary>
        /// Знищує всі GameObject-и пулу та очищає внутрішні колекції.
        /// </summary>
        public void Dispose()
        {
            foreach (var view in _active.Where(view => view != null))
                UnityEngine.Object.Destroy(view.gameObject);

            _active.Clear();

            foreach (var view in _free.Values.SelectMany(stack => stack.Where(view => view != null)))
                UnityEngine.Object.Destroy(view.gameObject);

            _free.Clear();
            _prefabOf.Clear();
        }
    }
}
