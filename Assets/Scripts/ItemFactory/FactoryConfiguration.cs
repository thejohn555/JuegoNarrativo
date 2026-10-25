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
        Buton
    }
    public TypeInteractive typeName;
    public string objectName;
    public string prefabName;
    public Sprite sprite;
    public Vector2 worldPosition;
    public float worldWidth = 1f;
    public float worldHeight = 1f;
    public Color spriteTint = Color.white;

    public bool addCollider = true;
    public bool colliderIsTrigger;
    public bool addRigidbody;
    public RigidbodyType2D bodyType = RigidbodyType2D.Static;

    public bool addOutliner;
    public Color outlineColor = Color.white;
    public float outlineScale = 1.1f;

    public Type componentType;
    public bool savePrefab;
    public string prefabFolder = "Assets/Prefabs/Generated";
}
