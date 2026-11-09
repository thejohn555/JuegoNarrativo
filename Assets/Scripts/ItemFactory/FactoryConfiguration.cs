using System;
using UnityEngine;

[Serializable]
public class FactoryConfiguration
{
    public enum TypeInteractive
    {
        InteractObject,
        collectableObject,
        NPC,
        Buton,
        Trigger,
        Decoration,
        Door
    }
    
    public TypeInteractive typeName;
    public string objectName;
    public string prefabName;
    public Sprite sprite;
    public Vector2 worldPosition;
    public float worldZ = -1f;
    public float worldWidth = 1f;
    public float worldHeight = 1f;
    public Color spriteTint = Color.white;
    public string sortingLayerName = "Default";
    public int sortingOrder = 1;

    public bool addCollider = true;
    public bool colliderIsTrigger;
    public bool addRigidbody;
    public RigidbodyType2D bodyType = RigidbodyType2D.Static;

    public bool addOutliner;
    public Color outlineColor = Color.white;
    public Color interactionOutlineColor = Color.yellow;
    public float outlineScale = 1.1f;
    public float outlineinteractionDistane = 2f;
    public Vector2 visualOffset = Vector2.zero;
    public RuntimeDialogueGraph interactiveDescription;
    public bool includeInNodeGraph;

    public Type componentType;
    public bool savePrefab;
    public string prefabFolder = "Assets/Prefabs/Generated";
}
