using Audio.Enums;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(AudioBank))]
public class AudioBankEditor : Editor
{
    #region SerializedProperties
    SerializedProperty audios;
    SerializedProperty soundOptions;
    SerializedProperty varOptions;
    SerializedProperty enumOptions;
    SerializedProperty loop;

    private void OnEnable()
    {
        audios = serializedObject.FindProperty("audios");
        soundOptions = serializedObject.FindProperty("soundOptions");
        varOptions = serializedObject.FindProperty("varOptions");
        enumOptions = serializedObject.FindProperty("enumOptions");
        loop = serializedObject.FindProperty("loop");
    }

    #endregion

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(audios);
        EditorGUILayout.PropertyField(soundOptions);
        

        if(soundOptions.enumValueIndex == (int)SoundOptions.Variable)
        {
            EditorGUILayout.PropertyField(varOptions);
            if(varOptions.enumValueIndex == (int)VarOptions.Enum)
            {
                EditorGUILayout.PropertyField(enumOptions);
            }
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
