using System;
using UnityEngine;

namespace FTg.Common
{
    /// <summary>
    /// Используется на [SerializeReference]-полях.
    /// Показывает выпадающий список производных типов и создаёт экземпляр выбранного типа.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SelectInstanceAttribute : PropertyAttribute
    {
        /// <summary>Базовый тип для фильтрации списка. Если null — берётся из типа поля.</summary>
        public Type BaseType { get; }

        public SelectInstanceAttribute(Type baseType = null)
        {
            BaseType = baseType;
        }
    }
}

