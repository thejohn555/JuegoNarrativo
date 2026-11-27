#if UNITY_EDITOR
using System;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using DoorGraph;
using UnityEditor;
using UnityEngine;

namespace ItemFactory
{
    public abstract class FactoryTypeOptions
    {
        public abstract Type ComponentType { get; }

        public virtual void DrawOptions()
        {
        }

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

    public sealed class DoorFactoryTypeOptions : FactoryTypeOptions
    {
        private int destinationSceneIndex = -1;

        public override Type ComponentType => typeof(Interactives.Door);

        public override void DrawOptions()
        {
            EditorGUILayout.LabelField("Door Settings", EditorStyles.boldLabel);
            destinationSceneIndex = EditorGUILayout.IntField("Destination scene index", destinationSceneIndex);
            EditorGUILayout.HelpBox(
                "Enter the destination scene's build index.",
                MessageType.Info);
        }

        public override bool Validate(out string error)
        {
            if (destinationSceneIndex < 0)
            {
                error = "Enter a destination scene build index of 0 or greater.";
                return false;
            }

            error = null;
            return true;
        }

        public override void ConfigureCreatedObject(GameObject target)
        {
            if (target == null)
                return;

            Interactives.Door door = target.GetComponent<Interactives.Door>();
            if (door != null)
            {
                SerializedObject serializedDoor = new SerializedObject(door);
                SerializedProperty destinationProperty =
                    serializedDoor.FindProperty("<DestinationScenePath>k__BackingField");
                if (destinationProperty != null)
                {
                    Undo.RecordObject(door, "Set door destination scene");
                    destinationProperty.intValue = destinationSceneIndex;
                    serializedDoor.ApplyModifiedProperties();
                    EditorUtility.SetDirty(door);
                }
                else
                {
                    Debug.LogWarning("Factory: Door.DestinationScenePath was not found.", door);
                }
            }

            DoorManager doorManager = DoorManager.Instance;
            if (doorManager == null)
            {
                Debug.LogWarning("Factory: no DoorManager instance was found; the door was not added to the Door Graph.", target);
                return;
            }

            Undo.RecordObject(doorManager, "Add door to Door Graph");
            doorManager.AddDoor(target);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(doorManager.gameObject.scene);
        }
    }


    public sealed class TriggerFactoryTypeOptions : FactoryTypeOptions
    {
        private bool includeInNodeGraph;

        public override Type ComponentType => typeof(Interactives.Trigger);
        public override void DrawOptions()
        {
            includeInNodeGraph = EditorGUILayout.Toggle("Include in NodeGraph", includeInNodeGraph);
        }

        public override void ConfigureCreatedObject(GameObject target)
        {
            if (includeInNodeGraph)
            {
                Debug.Log("Incluyendo en node Graph");
                DicitionaryFuntions.AddToDictionary(NodeTypes.ReferenceTrigger, target);
            }

            DicitionaryFuntions.AddToDictionary(NodeTypes.Trigger, target);

        }
        
    }

    public sealed class DecorationFactoryTypeOptions : FactoryTypeOptions
    {
        private bool includeInNodeGraph;

        public override Type ComponentType => typeof(Interactives.Decoration);

        public override void DrawOptions()
        {
            includeInNodeGraph = EditorGUILayout.Toggle("Include in NodeGraph", includeInNodeGraph);
        }

        public override void ConfigureCreatedObject(GameObject target)
        {
            if (!includeInNodeGraph) return;
            DicitionaryFuntions.AddToDictionary(NodeTypes.ReferenceGo, target);
        }
    }


    public sealed class InteractObjectFactoryTypeOptions : FactoryTypeOptions
    {
        public override Type ComponentType => typeof(Interactives.InteractObject);
        private bool includeInNodeGraph;

        public override void DrawOptions()
        {
            includeInNodeGraph = EditorGUILayout.Toggle("Include in NodeGraph", includeInNodeGraph);
        }

        public override void ConfigureCreatedObject(GameObject target)
        {
            DicitionaryFuntions.AddToDictionary(NodeTypes.Interactable, target);
            if (includeInNodeGraph)
            {
                DicitionaryFuntions.AddToDictionary(NodeTypes.ReferenceInteractable, target);
            }
        }
    }

    public sealed class NpcFactoryTypeOptions : FactoryTypeOptions
    {
        private string npcID = string.Empty;
        private bool hasDialogues;
        private RuntimeDialogueGraph currentGraph;

        private readonly System.Collections.Generic.List<Interactives.DialogueState> alternativeStates =
            new System.Collections.Generic.List<Interactives.DialogueState>();

        public override Type ComponentType => typeof(Interactives.NPC);
        private bool includeInNodeGraph;


        public override void DrawOptions()
        {
            includeInNodeGraph = EditorGUILayout.Toggle("Include in NodeGraph", includeInNodeGraph);
            EditorGUILayout.LabelField("NPC Settings", EditorStyles.boldLabel);
            npcID = EditorGUILayout.TextField("NPC ID", npcID);
            hasDialogues = EditorGUILayout.Toggle("Has dialogues", hasDialogues);
            if (!hasDialogues)
                return;

            currentGraph = (RuntimeDialogueGraph)EditorGUILayout.ObjectField(
                "Current graph", currentGraph, typeof(RuntimeDialogueGraph), false);
            if (currentGraph == null)
                EditorGUILayout.HelpBox("Assign a RuntimeDialogueGraph for this NPC.", MessageType.Warning);

            EditorGUILayout.LabelField("Alternative states", EditorStyles.boldLabel);
            for (int i = 0; i < alternativeStates.Count; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                Interactives.DialogueState state = alternativeStates[i];
                state.StateID = EditorGUILayout.TextField("State ID", state.StateID);
                state.DialogueGraph = (RuntimeDialogueGraph)EditorGUILayout.ObjectField(
                    "Dialogue graph", state.DialogueGraph, typeof(RuntimeDialogueGraph), false);
                alternativeStates[i] = state;
                if (GUILayout.Button("Remove state"))
                {
                    alternativeStates.RemoveAt(i);
                    i--;
                }

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add alternative state"))
                alternativeStates.Add(new Interactives.DialogueState());


        }

        public override bool Validate(out string error)
        {
            if (string.IsNullOrWhiteSpace(npcID))
            {
                error = "Enter an NPC ID.";
                return false;
            }

            if (hasDialogues && currentGraph == null)
            {
                error = "Assign a current graph or disable the dialogue option.";
                return false;
            }

            error = null;
            return true;
        }

        public override void ConfigureCreatedObject(GameObject target)
        {
            Interactives.NPC npc = target.GetComponent<Interactives.NPC>();
            if (npc == null)
            {
                Debug.LogWarning("Factory: the NPC component was not found.", target);
                return;
            }

            DicitionaryFuntions.AddToDictionary(NodeTypes.InteractableNpc, target);
            if (includeInNodeGraph)
            {
                DicitionaryFuntions.AddToDictionary(NodeTypes.ReferenceNpc, target);
            }

            SerializedObject serializedNpc = new SerializedObject(npc);
            SerializedProperty npcIdProperty = serializedNpc.FindProperty("npcID");
            SerializedProperty graphProperty = serializedNpc.FindProperty("currentGraph");
            SerializedProperty statesProperty = serializedNpc.FindProperty("alternativeStates");
            if (npcIdProperty == null || graphProperty == null || statesProperty == null)
            {
                Debug.LogWarning("Factory: one or more serialized NPC fields could not be found.", target);
                return;
            }

            Undo.RecordObject(npc, "Configure NPC");
            npcIdProperty.stringValue = npcID.Trim();
            graphProperty.objectReferenceValue = hasDialogues ? currentGraph : null;
            statesProperty.arraySize = hasDialogues ? alternativeStates.Count : 0;
            for (int i = 0; i < statesProperty.arraySize; i++)
            {
                SerializedProperty stateProperty = statesProperty.GetArrayElementAtIndex(i);
                stateProperty.FindPropertyRelative("StateID").stringValue = alternativeStates[i].StateID;
                stateProperty.FindPropertyRelative("DialogueGraph").objectReferenceValue =
                    alternativeStates[i].DialogueGraph;
            }

            serializedNpc.ApplyModifiedProperties();
            EditorUtility.SetDirty(npc);

        }
    }

    public sealed class CollectableObjectFactoryTypeOptions : FactoryTypeOptions
    {
        private bool includeInNodeGraph;
        public override Type ComponentType => typeof(Interactives.CollectableObject);

        private bool isCrafted;
        private bool iconOnInventory;
        private Sprite iconInventory;
        private GameObject ingredientA;
        private GameObject ingredientB;

        public override void DrawOptions()
        {
            EditorGUILayout.LabelField("Collectible Settings", EditorStyles.boldLabel);
            includeInNodeGraph = EditorGUILayout.Toggle("Include in NodeGraph", includeInNodeGraph);

            iconOnInventory = EditorGUILayout.Toggle("Change Inventory Icon?", iconOnInventory);

            if (iconOnInventory)
            {
               EditorGUILayout.Space();
               iconInventory = (Sprite)EditorGUILayout.ObjectField("Inventory Icon", iconInventory, typeof(Sprite));
            }
            
            isCrafted = EditorGUILayout.Toggle("Is this a craftable object?", isCrafted);

            if (isCrafted)
            { 
                EditorGUILayout.Space();
                
                ingredientA = (GameObject)EditorGUILayout.ObjectField("Ingredient A", ingredientA, typeof(GameObject), false);
                ingredientB = (GameObject)EditorGUILayout.ObjectField("Ingredient B", ingredientB, typeof(GameObject), false);

                if (ingredientA == null)
                {
                    EditorGUILayout.HelpBox("Assign Ingredient A to create this crafted item.", MessageType.Warning);
                }

                if (ingredientB == null)
                {
                    EditorGUILayout.HelpBox("Assign Ingredient B to create this crafted item.", MessageType.Warning);
                }
            }
        }

        public override bool Validate(out string error)
        {
            if (isCrafted && (ingredientA == null || ingredientB == null))
            {
                error = "Assign both ingredients for the crafted item or disable this option.";
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
                Debug.LogWarning("Factory: the collectible component was not found.", target);
                return;
            }
            if (!includeInNodeGraph) return;
            DicitionaryFuntions.AddToDictionary(NodeTypes.ReferenceGo, target);
        
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
    public static class DicitionaryFuntions
    {
        public static void AddToDictionary(NodeTypes type, GameObject target)
        {
            var manager = UnityEngine.Object.FindFirstObjectByType<BlueprintDictionaryManager>();

            if (manager == null)
            {
                Debug.LogWarning("Factory: no BlueprintDictionaryManager was found in the scene.", target);
                return;
            }

            manager.AddToList(target, type);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(target.scene);
        }
    }
}
#endif
