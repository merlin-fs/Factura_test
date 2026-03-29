using System.Collections.Generic;

namespace Game.Core.Common
{
    /// <summary>
    /// Registers ITickSystem implementations and drives their Tick() call each frame.
    /// Supports safe Register/Unregister from within a Tick (deferred apply).
    /// </summary>
    public sealed class TickSystemRegistry
    {
        private readonly List<ITickSystem> _systems       = new();
        private readonly List<ITickSystem> _pendingAdd    = new();
        private readonly List<ITickSystem> _pendingRemove = new();

        private bool _isTicking;

        public void Register(ITickSystem system)
        {
            if (_isTicking)
                _pendingAdd.Add(system);
            else
                _systems.Add(system);
        }

        public void Unregister(ITickSystem system)
        {
            if (_isTicking)
                _pendingRemove.Add(system);
            else
                _systems.Remove(system);
        }

        /// <summary>Tick all registered systems then apply deferred adds/removes.</summary>
        public void Tick(float dt)
        {
            _isTicking = true;
            foreach (var t in _systems)
                t.Tick(dt);

            _isTicking = false;

            // Apply deferred removes first to avoid stale references
            foreach (var t in _pendingRemove)
                _systems.Remove(t);

            _pendingRemove.Clear();

            foreach (var t in _pendingAdd)
                _systems.Add(t);

            _pendingAdd.Clear();
        }

        /// <summary>Remove all registrations (call on session end).</summary>
        public void Clear()
        {
            _systems.Clear();
            _pendingAdd.Clear();
            _pendingRemove.Clear();
        }
    }
}

