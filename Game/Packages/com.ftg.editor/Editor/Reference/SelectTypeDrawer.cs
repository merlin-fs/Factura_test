using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FTg.Common.Editor
{
    [CustomPropertyDrawer(typeof(SelectTypeAttribute), true)]
    class SelectTypeDrawer : BaseReferenceDrawer
    {
        protected override void GetDisplayValue(object value, ref string display)
        {
            display = (string)value ?? "(null)";
        }

        protected override void OnSelect(SerializedProperty property, Type type)
        {
            var attr = (SelectTypeAttribute)attribute;
            
            var value = (attr.AssemblyQualifiedName) 
                ? type?.AssemblyQualifiedName
                : type?.FullName;
            if (attr.GetterMethodName != null && attr.GetterOwnerType != null)
            {
                var getter = SelectTypeAttributeUtils.ResolveGetter(attr.GetterOwnerType, attr.GetterMethodName);
                value = getter.Invoke(type);
            }
                
            property.stringValue = value ?? null;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        protected override Type GetBaseType(SerializedProperty property)
        {
            SelectTypeAttribute attr = (SelectTypeAttribute)attribute;
            var type = attr.SelectType;
            return type;
        }

        protected override Rect GetRect(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            return EditorGUI.PrefixLabel(position, label); 
        }
    }
    
    public static class SelectTypeAttributeUtils
    {
        private static readonly Dictionary<string, Func<Type, string>> Cache = new();

        // Ожидается статический метод: string Method(Type t)
        public static Func<Type, string> ResolveGetter(Type ownerType, string methodName)
        {
            if (ownerType == null || string.IsNullOrEmpty(methodName))
                return null;

            var key = ownerType.FullName + "#" + methodName;
            if (Cache.TryGetValue(key, out var cached))
                return cached;

            var mi = ownerType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]{typeof(Type)}, null);
            if (mi == null)
                return null;

            var parameters = mi.GetParameters();
            if (mi.ReturnType != typeof(string) || parameters.Length != 1 || parameters[0].ParameterType != typeof(Type))
                return null;

            Func<Type, string> func = (Type t) =>
            {
                try
                {
                    return (string)mi.Invoke(null, new object[] { t });
                }
                catch
                {
                    return null;
                }
            };

            Cache[key] = func;
            return func;
        }
    }    
}
