using System;
using UnityEngine;

namespace FTg.Common
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SelectTypeAttribute : PropertyAttribute
    {
        public bool AssemblyQualifiedName { get; } = false;
        public Type SelectType { get; }
        public Type GetterOwnerType { get; }
        public string GetterMethodName { get; }

        public SelectTypeAttribute(Type selectType, bool assemblyQualifiedName = false)
        {
            SelectType = selectType;
            AssemblyQualifiedName = assemblyQualifiedName;
            GetterMethodName = null;
            GetterOwnerType = null;
        }

        public SelectTypeAttribute(Type selectType, Type getterOwnerType, string getterMethodName)
        {
            SelectType = selectType;
            AssemblyQualifiedName = false;
            GetterMethodName = getterMethodName;
            GetterOwnerType = getterOwnerType;
        }
    }
}