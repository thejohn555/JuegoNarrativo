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

    private void OnEnable()
    {
        eventBus = serializedObject.FindProperty("eventBus");
        sfx = serializedObject.FindProperty("sfx");
        soundOptions = serializedObject.FindProperty("soundOptions");
        varOptions = serializedObject.FindProperty("varOptions");
        loop = serializedObject.FindProperty("loop");
    }

    #endregion

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(eventBus); 
        EditorGUILayout.PropertyField(sfx);
        EditorGUILayout.PropertyField(soundOptions);
        

        //if(soundOptions.enumValueIndex == (int)SoundOptions.Variable)
        //{
        //    EditorGUILayout.PropertyField(varOptions);
        //}
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
