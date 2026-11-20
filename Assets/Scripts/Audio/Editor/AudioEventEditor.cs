using Audio.Enums;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(AudioEvent))]
public class AudioEventEditor : Editor
{
    #region SerializedProperties
    SerializedProperty eventBus;
    SerializedProperty sfx;
    SerializedProperty soundOptions;
    SerializedProperty varOptions;
    SerializedProperty loop;
    SerializedProperty usingAudioBank;
    SerializedProperty bankType;

    private void OnEnable()
    {
        sfx = serializedObject.FindProperty("sfx");
        soundOptions = serializedObject.FindProperty("soundOptions");
        varOptions = serializedObject.FindProperty("varOptions");
        loop = serializedObject.FindProperty("loop");
        usingAudioBank = serializedObject.FindProperty("usingAudioBank");
        bankType = serializedObject.FindProperty("bankType");
    }

    #endregion

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(usingAudioBank);
        if(usingAudioBank.boolValue == true)
        {
            EditorGUILayout.PropertyField(bankType);
        }
        else
        {
            EditorGUILayout.PropertyField(sfx);
        }
        EditorGUILayout.PropertyField(soundOptions);
        EditorGUILayout.PropertyField(loop);

        //if(soundOptions.enumValueIndex == (int)SoundOptions.Variable)
        //{
        //    EditorGUILayout.PropertyField(varOptions);
        //}
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
