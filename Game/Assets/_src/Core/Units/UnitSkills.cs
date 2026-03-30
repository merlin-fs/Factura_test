using System;
using System.Collections.Generic;

namespace Game.Core.Units
{
    /// <summary>
    /// Типізований контейнер навичок юніта, що виконується під час гри.
    /// Будується один раз із <see cref="UnitConfig.CreateSkills"/> і зберігається в <see cref="Unit"/>.
    /// </summary>
    public sealed class UnitSkills
    {
        private readonly Dictionary<Type, ISkill> _skills = new();
        private readonly List<ISkill>             _unique = new();

        internal UnitSkills(IEnumerable<ISkill> skills)
        {
            foreach (var s in skills)
                Add(s.GetType(), s);
        }

        /// <summary>
        /// Додає навичку до контейнера за типом.
        /// </summary>
        public void Add<TSkill>(TSkill skill) where TSkill : class, ISkill
            => Add(typeof(TSkill), skill);

        /// <summary>Усі унікальні екземпляри навичок — для пакетного впровадження залежностей у конструкторі юніта.</summary>
        public IReadOnlyList<ISkill> All => _unique;

        private void Add(Type concreteType, ISkill skill)
        {
            _skills[concreteType] = skill ?? throw new ArgumentNullException(nameof(skill));
            _unique.Add(skill);
            RegisterSkillInterfaces(skill);
        }

        /// <summary>
        /// Повертає навичку за типом. Кидає виняток, якщо навичка не зареєстрована.
        /// </summary>
        /// <typeparam name="TSkill">Тип навички.</typeparam>
        public TSkill Get<TSkill>() where TSkill : class, ISkill
        {
            if (_skills.TryGetValue(typeof(TSkill), out var skill))
                return (TSkill)skill;
            throw new InvalidOperationException($"Skill '{typeof(TSkill).Name}' is not registered on unit.");
        }

        /// <summary>
        /// Перевіряє, чи зареєстрована навичка вказаного типу.
        /// </summary>
        /// <typeparam name="TSkill">Тип навички.</typeparam>
        public bool Has<TSkill>() where TSkill : class, ISkill
            => _skills.ContainsKey(typeof(TSkill));

        private void RegisterSkillInterfaces(ISkill skill)
        {
            var concreteType = skill.GetType();
            foreach (var type in concreteType.GetInterfaces())
            {
                if (type == typeof(ISkill)) continue;
                if (!typeof(ISkill).IsAssignableFrom(type)) continue;
                _skills[type] = skill;
            }
        }
    }
}
