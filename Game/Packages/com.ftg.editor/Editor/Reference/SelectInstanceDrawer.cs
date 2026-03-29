using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FTg.Common.Editor
{
    [CustomPropertyDrawer(typeof(SelectInstanceAttribute), true)]
    class SelectInstanceDrawer : BaseReferenceDrawer
    {
        protected override Type GetBaseType(SerializedProperty property)
        {
            var attr = (SelectInstanceAttribute)attribute;
            var fieldType = TypeHelper.GetRealTypeFromTypename(property.managedReferenceFieldTypename);
            return attr.BaseType ?? fieldType;
        }

        protected override void OnSelect(SerializedProperty property, Type type)
        {
            property.managedReferenceValue = (type != null) ? Activator.CreateInstance(type) : null;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        protected override void FinalizeProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        protected override Rect GetRect(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            position.x += EditorGUIUtility.labelWidth + EditorGUI.indentLevel * 15 + 3;
            position.width -= EditorGUIUtility.labelWidth + 4;
            return position;
        }
    }
}

