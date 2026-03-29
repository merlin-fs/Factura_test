using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Units
{
    public abstract class Unit
    {
        public readonly UnitStats Stats;
        public readonly UnitSkills Skills;
        public readonly LayerMask TargetMask;

        public abstract Vector3 Position { get; }

        protected Unit(UnitConfig config)
        {
            Stats = new UnitStats(config.CreateStats());
            Skills = new UnitSkills(config.CreateSkills());
            TargetMask = config.TargetMask;
        }

        public sealed class UnitStats
        {
            private readonly Dictionary<Type, UnitStat> _items = new();

            internal UnitStats(IEnumerable<UnitStat> stats)
            {
                foreach (var s in stats)
                    _items[s.GetType()] = s;
            }

            public T Get<T>() where T : UnitStat
            {
                if (_items.TryGetValue(typeof(T), out var s))
                    return (T)s;
                throw new InvalidOperationException($"Stat '{typeof(T).Name}' is not registered on unit.");
            }

            public bool Has<T>() where T : UnitStat => _items.ContainsKey(typeof(T));
        }

        public sealed class UnitSkills
        {
            private readonly Dictionary<Type, ISkill> _skills = new();

            internal UnitSkills(IEnumerable<ISkill> skills)
            {
                foreach (var s in skills)
                    Add(s.GetType(), s);
            }

            public void Add<TSkill>(TSkill skill)
                where TSkill : class, ISkill
            {
                Add(typeof(TSkill), skill);
            }

            private void Add(Type concreteType, ISkill skill)
            {
                _skills[concreteType] = skill ?? throw new ArgumentNullException(nameof(skill));
                RegisterSkillInterfaces(skill);
            }

            public TSkill Get<TSkill>()
                where TSkill : class, ISkill
            {
                if (_skills.TryGetValue(typeof(TSkill), out var skill))
                    return (TSkill)skill;

                throw new InvalidOperationException(
                    $"Skill '{typeof(TSkill).Name}' is not registered on unit.");
            }

            public bool Has<TSkill>()
                where TSkill : class, ISkill
            {
                return _skills.ContainsKey(typeof(TSkill));
            }

            private void RegisterSkillInterfaces(ISkill skill)
            {
                var concreteType = skill.GetType();
                var interfaces = concreteType.GetInterfaces();
                foreach (var type in interfaces)
                {
                    if (type == typeof(ISkill))
                        continue;
                    if (!typeof(ISkill).IsAssignableFrom(type))
                        continue;
                    _skills[type] = skill;
                }
            }
        }
    }
}
