#if UNITY_EDITOR
using System;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
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

    public virtual void ConfigureCreatedObject(GameObject target)
    {
        
    }
}

public sealed class EmptyFactoryTypeOptions : FactoryTypeOptions
{
    public override Type ComponentType => null;
}

public sealed class InteractObjectFactoryTypeOptions : FactoryTypeOptions
{
    public override Type ComponentType => typeof(Interactives.InteractObject);
    // esto esta aqui de forma temporal cuando este el crear trigger lo pongo donde toca
    public override void ConfigureCreatedObject(GameObject target)
    {
        var manager = UnityEngine.Object.FindFirstObjectByType<
            Blueprints_arquitectura_Dani_.Runtime.Managers.BlueprintDictionaryManager>();

        if (manager == null)
        {
            Debug.LogWarning("Factory: no hay BlueprintDictionaryManager en la escena.", target);
            return;
        }

        manager.AddToList(target,NodeTypes.Trigger);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(target.scene);
    }
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
        {
            return;
        }

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

public sealed class CollectableObjectFactoryTypeOptions : FactoryTypeOptions
{
    public override Type ComponentType => typeof(Interactives.CollectableObject);
    
    private bool isCrafted;
    private GameObject ingredientA;
    private GameObject ingredientB;
    
    public override void DrawOptions()
    {
        EditorGUILayout.LabelField("Collectibles Configuration", EditorStyles.boldLabel);
        isCrafted = EditorGUILayout.Toggle("The item is a craftable object.", isCrafted);
        
        if (!isCrafted)
        {
            return;  
        }

        ingredientA = (GameObject)EditorGUILayout.ObjectField("ingredientA: ", ingredientA, typeof(GameObject), false);
        ingredientB = (GameObject)EditorGUILayout.ObjectField("ingredientB: ", ingredientB, typeof(GameObject), false);
        
        if (ingredientA == null)
        {
            EditorGUILayout.HelpBox("Assign an ingredient A to create this crafted item.", MessageType.Warning);
        }
        
        if (ingredientB == null)
        {
            EditorGUILayout.HelpBox("Assign an ingredient B to create this crafted item.", MessageType.Warning);
        }
    }

    public override bool Validate(out string error)
    {
        if (isCrafted && (ingredientA == null || ingredientB == null))
        {
            error = "You must assign both ingredients for the crafted item or disable the option.";
            return false;
        }
        
        error = null;
        return true;
    }

    public override void ConfigureCreatedObject(GameObject target)
    {
        Interactives.CollectableObject collectableObject = target.GetComponent<Interactives.CollectableObject>();
        if (collectableObject == null)
        {
            Debug.LogWarning("Factory: the collectable component was not found.", target);
            return;
        }

        if (isCrafted)
        {
            SerializedObject serializedCollectable = new SerializedObject(collectableObject);
            SerializedProperty propA = serializedCollectable.FindProperty("ingredientA");
            SerializedProperty propB = serializedCollectable.FindProperty("ingredientB");
            
            if (propA != null) propA.objectReferenceValue = ingredientA;
            if (propB != null) propB.objectReferenceValue = ingredientB;

            Undo.RecordObject(collectableObject, "Assign ingredients");
            serializedCollectable.ApplyModifiedProperties();
            EditorUtility.SetDirty(collectableObject);
        }
    }
}
#endif
