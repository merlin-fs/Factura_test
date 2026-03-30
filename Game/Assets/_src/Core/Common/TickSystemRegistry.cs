using System.Collections.Generic;

namespace Game.Core.Common
{
    /// <summary>
    /// Реєстр систем <see cref="ITickSystem"/>. Викликає метод <see cref="ITickSystem.Tick"/> кожен кадр.
    /// Підтримує безпечну реєстрацію та скасування реєстрації під час виконання Tick (відкладене застосування).
    /// </summary>
    public sealed class TickSystemRegistry
    {
        private readonly List<ITickSystem> _systems       = new();
        private readonly List<ITickSystem> _pendingAdd    = new();
        private readonly List<ITickSystem> _pendingRemove = new();

        private bool _isTicking;

        /// <summary>
        /// Реєструє систему для отримання викликів Tick.
        /// Якщо реєстрація відбувається під час Tick — відкладається до кінця кадру.
        /// </summary>
        /// <param name="system">Система для реєстрації.</param>
        public void Register(ITickSystem system)
        {
            if (_isTicking)
                _pendingAdd.Add(system);
            else
                _systems.Add(system);
        }

        /// <summary>
        /// Скасовує реєстрацію системи.
        /// Якщо скасування відбувається під час Tick — відкладається до кінця кадру.
        /// </summary>
        /// <param name="system">Система для видалення.</param>
        public void Unregister(ITickSystem system)
        {
            if (_isTicking)
                _pendingRemove.Add(system);
            else
                _systems.Remove(system);
        }

        /// <summary>
        /// Викликає Tick у всіх зареєстрованих систем, потім застосовує відкладені зміни.
        /// </summary>
        /// <param name="dt">Дельта-час у секундах.</param>
        public void Tick(float dt)
        {
            _isTicking = true;
            foreach (var t in _systems)
                t.Tick(dt);

            _isTicking = false;

            foreach (var t in _pendingRemove)
                _systems.Remove(t);

            _pendingRemove.Clear();

            foreach (var t in _pendingAdd)
                _systems.Add(t);

            _pendingAdd.Clear();
        }

        /// <summary>
        /// Видаляє всі реєстрації. Викликати наприкінці сесії.
        /// </summary>
        public void Clear()
        {
            _systems.Clear();
            _pendingAdd.Clear();
            _pendingRemove.Clear();
        }
    }
}
