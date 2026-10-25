#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public abstract class FactoryTypeOptions
{
    public abstract Type ComponentType { get; }

    public virtual void DrawOptions() { }

    public virtual bool Validate(out string error)
    {
        error = null;
        return true;
    }

    public virtual void ConfigureCreatedObject(GameObject target) { }
}

public sealed class EmptyFactoryTypeOptions : FactoryTypeOptions
{
    public override Type ComponentType => null;
}

public sealed class InteractObjectFactoryTypeOptions : FactoryTypeOptions
{
    public override Type ComponentType => typeof(Interactives.InteractObject);
}

public sealed class NpcFactoryTypeOptions : FactoryTypeOptions
{
    private bool assignDialogueGraph;
    private RuntimeDialogueGraph dialogueGraph;

    public override Type ComponentType => typeof(Interactives.NPC);

    public override void DrawOptions()
    {
        EditorGUILayout.LabelField("Configuracion de NPC", EditorStyles.boldLabel);
        assignDialogueGraph = EditorGUILayout.Toggle("Asignar grafo de dialogo", assignDialogueGraph);
        if (!assignDialogueGraph)
            return;

        dialogueGraph = (RuntimeDialogueGraph)EditorGUILayout.ObjectField(
            "Grafo de dialogo", dialogueGraph, typeof(RuntimeDialogueGraph), false);
        if (dialogueGraph == null)
            EditorGUILayout.HelpBox("Asigna un RuntimeDialogueGraph para crear este NPC.", MessageType.Warning);
    }

    public override bool Validate(out string error)
    {
        if (assignDialogueGraph && dialogueGraph == null)
        {
            error = "Asigna un RuntimeDialogueGraph o desactiva esa opcion.";
            return false;
        }

        error = null;
        return true;
    }

    public override void ConfigureCreatedObject(GameObject target)
    {
        if (!assignDialogueGraph)
            return;

        Interactives.NPC npc = target.GetComponent<Interactives.NPC>();
        if (npc == null)
        {
            Debug.LogWarning("Factory: no se encontro el componente NPC para asignar el grafo.", target);
            return;
        }

        SerializedObject serializedNpc = new SerializedObject(npc);
        SerializedProperty dialogueProperty = serializedNpc.FindProperty("Dialogue");
        if (dialogueProperty == null)
        {
            Debug.LogWarning("Factory: NPC no contiene el campo serializado Dialogue.", target);
            return;
        }

        Undo.RecordObject(npc, "Assign NPC dialogue graph");
        dialogueProperty.objectReferenceValue = dialogueGraph;
        serializedNpc.ApplyModifiedProperties();
        EditorUtility.SetDirty(npc);
    }
}
#endif
