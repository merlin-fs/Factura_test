using System;
using System.Collections.Generic;

namespace Game.Core.Units
{
    /// <summary>
    /// Типізований контейнер статистик юніта (HP, швидкість тощо), що виконується під час гри.
    /// Будується один раз із <see cref="UnitConfig.CreateStats"/> і зберігається в <see cref="Unit"/>.
    /// </summary>
    public sealed class UnitStats
    {
        private readonly Dictionary<Type, UnitStat> _items  = new();
        private readonly List<UnitStat>             _unique = new();

        internal UnitStats(IEnumerable<UnitStat> stats)
        {
            foreach (var s in stats)
            {
                _items[s.GetType()] = s;
                _unique.Add(s);
            }
        }

        /// <summary>Усі унікальні екземпляри статистик у порядку додавання.</summary>
        public IReadOnlyList<UnitStat> All => _unique;

        /// <summary>
        /// Повертає статистику за типом. Кидає виняток, якщо статистика не зареєстрована.
        /// </summary>
        /// <typeparam name="T">Тип статистики.</typeparam>
        public T Get<T>() where T : UnitStat
        {
            if (_items.TryGetValue(typeof(T), out var s))
                return (T)s;
            throw new InvalidOperationException($"Stat '{typeof(T).Name}' is not registered on unit.");
        }

        /// <summary>
        /// Перевіряє, чи зареєстрована статистика вказаного типу.
        /// </summary>
        /// <typeparam name="T">Тип статистики.</typeparam>
        public bool Has<T>() where T : UnitStat => _items.ContainsKey(typeof(T));
    }
}
