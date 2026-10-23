using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BlackboardSystem
{
    [CustomEditor(typeof(BlackboardData))]
    public class BlackboardDataEditor : Editor
    {
        private ReorderableList entryList;

        private void OnEnable()
        {
            entryList = new ReorderableList(serializedObject, serializedObject.FindProperty("entries"), true, true, true, true)
            {
                //anonymous method
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight), "Key");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight), "Type");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.6f + 5, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight), "Value");
                }
            };

            entryList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = entryList.serializedProperty.GetArrayElementAtIndex(index);

                rect.y += 2;
                SerializedProperty keyName = element.FindPropertyRelative("keyName");
                SerializedProperty valueType = element.FindPropertyRelative("valueType");
                SerializedProperty value = element.FindPropertyRelative("value");

                Rect keyNameRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                Rect valueTypeRect = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                Rect valueRect = new Rect(rect.x + rect.width * 0.6f, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(keyNameRect, keyName, GUIContent.none);
                EditorGUI.PropertyField(valueTypeRect, valueType, GUIContent.none);

                switch ((AnyValue.ValueType)valueType.enumValueIndex)
                {
                    case AnyValue.ValueType.Bool:
                        SerializedProperty boolValue = value.FindPropertyRelative("boolValue");
                        EditorGUI.PropertyField(valueRect, boolValue, GUIContent.none);
                        break;
                    case AnyValue.ValueType.Int:
                        SerializedProperty intValue = value.FindPropertyRelative("intValue");
                        EditorGUI.PropertyField(valueRect, intValue, GUIContent.none);
                        break;
                    case AnyValue.ValueType.Float:
                        SerializedProperty floatValue = value.FindPropertyRelative("floatValue");
                        EditorGUI.PropertyField(valueRect, floatValue, GUIContent.none);
                        break;
                    case AnyValue.ValueType.String:
                        SerializedProperty stringValue = value.FindPropertyRelative("stringValue");
                        EditorGUI.PropertyField(valueRect, stringValue, GUIContent.none);
                        break;
                    case AnyValue.ValueType.Vector3:
                        SerializedProperty vector3Value = value.FindPropertyRelative("vector3Value");
                        EditorGUI.PropertyField(valueRect, vector3Value, GUIContent.none);
                        break;
                    case AnyValue.ValueType.GameObject:
                        SerializedProperty gameObjectValue = value.FindPropertyRelative("gameObjectValue");
                        EditorGUI.PropertyField(valueRect, gameObjectValue, GUIContent.none);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            entryList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}