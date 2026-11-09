using System;
using System.Collections.Generic;
using System.Linq;
using ItemFactory;
using UnityEditor;
using UnityEngine;

public class FactoryWindow : EditorWindow
{
    public FactoryConfiguration.TypeInteractive selectedType;

    private string search = "";
    private string objectName = "GameObject";
    private string prefabName = "";
    private Vector2 spriteScroll;
    private Vector2 windowScroll;
    private Sprite selectedSprite;
    private bool includeSprite = true;
    private float worldZ = -1f;
    private string sortingLayerName = "Default";
    
    private float worldWidth = 1f;
    private float worldHeight = 1f;
    private Vector2 worldPosition = Vector2.zero;
    private int sortingOrder = 1;
    private Color spriteTint = Color.red;

    private readonly NpcFactoryTypeOptions npcOptions = new NpcFactoryTypeOptions();
    private readonly CollectableObjectFactoryTypeOptions collectableObjectOptions = new CollectableObjectFactoryTypeOptions();
    private readonly InteractObjectFactoryTypeOptions interactObjectOptions = new InteractObjectFactoryTypeOptions();
    private readonly TriggerFactoryTypeOptions triggerOptions = new TriggerFactoryTypeOptions();
    private readonly DecorationFactoryTypeOptions decorationOptions = new DecorationFactoryTypeOptions();
    private readonly DoorFactoryTypeOptions doorOptions = new DoorFactoryTypeOptions();
    private readonly EmptyFactoryTypeOptions emptyOptions = new EmptyFactoryTypeOptions();

    private bool addCollider = true;
    private bool colliderIsTrigger;
    private bool addRigidbody;
    private RigidbodyType2D bodyType = RigidbodyType2D.Static;
    private bool addOutliner;
    
    
    private Color outlineColor = Color.white;
    private Color interactionOutlineColor = Color.yellow;
    private float outlineScale = 1.1f;
    private float outlineinteractionDistane = 2f;
    private Vector2 visualOffset = Vector2.zero;
    private RuntimeDialogueGraph interactiveDescription;
    private bool includeInNodeGraph;

    private bool savePrefab;
    private string prefabFolder = "Assets/Prefabs/Generated";
    private List<Sprite> sprites = new List<Sprite>();

    [MenuItem("Tools/2D Object Factory")]
    public static void Open()
    {
        FactoryWindow window = GetWindow<FactoryWindow>();
        window.titleContent = new GUIContent("2D Factory");
        window.minSize = new Vector2(370, 620);
        window.RefreshSprites();
        window.Show();
    }

    private void OnEnable()
    {
        RefreshSprites();
    }

    private void OnGUI()
    {
        windowScroll = EditorGUILayout.BeginScrollView(windowScroll);
        EditorGUILayout.LabelField("2D Object Factory", EditorStyles.largeLabel);
        EditorGUILayout.HelpBox(
            "Create 2D objects in the scene with a sprite, world-space size, and configurable components.",
            MessageType.Info);

        EditorGUILayout.LabelField("Object name", EditorStyles.boldLabel);
        objectName = EditorGUILayout.TextField("In scene", objectName);
        EditorGUILayout.Space(6);
        DrawTypeSection();
        EditorGUILayout.Space(8);
        DrawTypeOptions();
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Sprite", EditorStyles.boldLabel);
        includeSprite = EditorGUILayout.Toggle("Include sprite", includeSprite);
        if (includeSprite)
            DrawSpriteBrowser();

        EditorGUILayout.Space(8);
        DrawTransformSection();
        EditorGUILayout.Space(8);
        DrawComponentsSection();
        EditorGUILayout.Space(8);
        DrawPrefabSection();
        EditorGUILayout.Space(12);

        bool needsSprite = includeSprite && selectedSprite == null;
        FactoryTypeOptions typeOptions = GetTypeOptions();
        bool invalidTypeOptions = typeOptions != null && !typeOptions.Validate(out _);
        using (new EditorGUI.DisabledScope(needsSprite || invalidTypeOptions))
        {
            if (GUILayout.Button("Create object in scene", GUILayout.Height(34)))
                CreateObject();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawTypeSection()
    {
        EditorGUILayout.LabelField("Object type", EditorStyles.boldLabel);
        string[] typeLabels = { "Interact Object", "Collectible Object", "NPC", "Button", "Trigger", "Decoration", "Door" };
        int selectedIndex = Mathf.Max(0, Array.IndexOf(
            (FactoryConfiguration.TypeInteractive[])Enum.GetValues(typeof(FactoryConfiguration.TypeInteractive)),
            selectedType));
        selectedIndex = EditorGUILayout.Popup("Type", selectedIndex, typeLabels);
        selectedType = (FactoryConfiguration.TypeInteractive)selectedIndex;
    }

    private void DrawTypeOptions()
    {
        FactoryTypeOptions typeOptions = GetTypeOptions();
        if (typeOptions != null)
        {
            typeOptions.DrawOptions();
        }
    }

    private void DrawSpriteBrowser()
    {
        EditorGUILayout.BeginHorizontal();
        string newSearch = EditorGUILayout.TextField("Search", search);
        if (newSearch != search)
        {
            search = newSearch;
            RefreshSprites();
        }

        if (GUILayout.Button("Refresh", GUILayout.Width(78)))
            RefreshSprites();
        EditorGUILayout.EndHorizontal();

        selectedSprite = (Sprite)EditorGUILayout.ObjectField(
            "Selected sprite", selectedSprite, typeof(Sprite), false);

        if (selectedSprite != null)
        {
            GUILayout.Label(AssetPreview.GetAssetPreview(selectedSprite) ??
                            AssetPreview.GetMiniThumbnail(selectedSprite),
                GUILayout.Width(100), GUILayout.Height(100));
            EditorGUILayout.LabelField("Name", selectedSprite.name);
            EditorGUILayout.LabelField("Original size",
                $"{selectedSprite.rect.width:0} x {selectedSprite.rect.height:0} px");
        }

        EditorGUILayout.LabelField($"Sprites found: {sprites.Count}", EditorStyles.miniLabel);
        spriteScroll = EditorGUILayout.BeginScrollView(spriteScroll, GUILayout.Height(160));
        const int columns = 4;
        int index = 0;

        while (index < sprites.Count)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < columns && index < sprites.Count; col++, index++)
            {
                Sprite sprite = sprites[index];
                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (selectedSprite == sprite)
                    style.normal = style.active;

                EditorGUILayout.BeginVertical(GUILayout.Width(76));
                Texture2D preview = AssetPreview.GetAssetPreview(sprite) ??
                                    AssetPreview.GetMiniThumbnail(sprite);
                if (GUILayout.Button(new GUIContent(preview, sprite.name), style,
                        GUILayout.Width(72), GUILayout.Height(72)))
                    SelectSprite(sprite);

                GUILayout.Label(sprite.name, EditorStyles.miniLabel,
                    GUILayout.Width(72), GUILayout.Height(28));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawTransformSection()
    {
        EditorGUILayout.LabelField("Size and position", EditorStyles.boldLabel);
        worldWidth = Mathf.Max(0.01f, EditorGUILayout.FloatField("Width (units)", worldWidth));
        worldHeight = Mathf.Max(0.01f, EditorGUILayout.FloatField("Height (units)", worldHeight));
        worldPosition = EditorGUILayout.Vector2Field("Position X / Y", worldPosition);
        worldZ = EditorGUILayout.FloatField("Position Z", worldZ);

        if (includeSprite)
        {
            string[] sortingLayerNames = SortingLayer.layers.Select(layer => layer.name).ToArray();
            if (sortingLayerNames.Length > 0)
            {
                int selectedLayerIndex = Array.IndexOf(sortingLayerNames, sortingLayerName);
                if (selectedLayerIndex < 0)
                    selectedLayerIndex = Array.IndexOf(sortingLayerNames, "Default");
                if (selectedLayerIndex < 0)
                    selectedLayerIndex = 0;

                selectedLayerIndex = EditorGUILayout.Popup("Sorting layer", selectedLayerIndex, sortingLayerNames);
                sortingLayerName = sortingLayerNames[selectedLayerIndex];
            }

            sortingOrder = EditorGUILayout.IntField("Sorting order", sortingOrder);
            spriteTint = EditorGUILayout.ColorField("Sprite tint", spriteTint);
            if (selectedSprite != null)
            {
                Vector2 size = GetSpriteWorldSize(selectedSprite);
                EditorGUILayout.LabelField("Estimated base size",
                    $"{size.x:0.###} x {size.y:0.###} units");
            }
        }
    }

    private void DrawComponentsSection()
    {
        EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);
        if (selectedType == FactoryConfiguration.TypeInteractive.Decoration)
            includeInNodeGraph = EditorGUILayout.Toggle("Include in NodeGraph", includeInNodeGraph);

        bool isTriggerType = selectedType == FactoryConfiguration.TypeInteractive.Trigger;
        if (isTriggerType)
        {
            EditorGUILayout.HelpBox("A trigger collider and its event relay will be added automatically.",
                MessageType.Info);
        }
        else
        {
            addCollider = EditorGUILayout.Toggle("Add Collider2D", addCollider);
            if (addCollider)
                colliderIsTrigger = EditorGUILayout.Toggle("Is trigger", colliderIsTrigger);
        }

        addRigidbody = EditorGUILayout.Toggle("Add Rigidbody2D", addRigidbody);
        if (addRigidbody)
            bodyType = (RigidbodyType2D)EditorGUILayout.EnumPopup("Rigidbody type", bodyType);

        addOutliner = EditorGUILayout.Toggle("Configure Outliner", addOutliner);
        if (addOutliner)
        {
            outlineColor = EditorGUILayout.ColorField("Outline color", outlineColor);
            interactionOutlineColor = EditorGUILayout.ColorField("Interaction outline color", interactionOutlineColor);
            outlineScale = EditorGUILayout.Slider("Outline scale", outlineScale, 1f, 1.5f);
        }

        FactoryTypeOptions typeOptions = GetTypeOptions();
        if (typeOptions != null &&
            typeof(Interactives.Interactive).IsAssignableFrom(typeOptions.ComponentType))
        {
            visualOffset = EditorGUILayout.Vector2Field("Visual offset (units)", visualOffset);
            outlineinteractionDistane = EditorGUILayout.FloatField(
                "Interaction distance", outlineinteractionDistane);
            interactiveDescription = (RuntimeDialogueGraph)EditorGUILayout.ObjectField(
                "Description graph (optional)", interactiveDescription, typeof(RuntimeDialogueGraph), false);
        }
    }

    private void DrawPrefabSection()
    {
        EditorGUILayout.LabelField("Optional prefab", EditorStyles.boldLabel);
        savePrefab = EditorGUILayout.Toggle("Save as prefab", savePrefab);
        using (new EditorGUI.DisabledScope(!savePrefab))
        {
            prefabName = EditorGUILayout.TextField("Prefab name", prefabName);
            prefabFolder = EditorGUILayout.TextField("Folder", prefabFolder);
            EditorGUILayout.HelpBox(
                "If the name is empty, the object name will be used. The folder will be created if needed and must be inside Assets.",
                MessageType.None);
        }
    }

    private void SelectSprite(Sprite sprite)
    {
        selectedSprite = sprite;
        Vector2 size = GetSpriteWorldSize(sprite);
        worldWidth = size.x;
        worldHeight = size.y;
        Repaint();
    }

    private Vector2 GetSpriteWorldSize(Sprite sprite)
    {
        if (sprite == null)
            return Vector2.one;

        float ppu = Mathf.Max(0.0001f, sprite.pixelsPerUnit);
        return new Vector2(sprite.rect.width / ppu, sprite.rect.height / ppu);
    }

    private void RefreshSprites()
    {
        sprites = AssetDatabase.FindAssets("t:Sprite")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Distinct()
            .SelectMany(path => AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>())
            .Where(sprite => sprite != null &&
                             (string.IsNullOrWhiteSpace(search) ||
                              sprite.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0))
            .OrderBy(sprite => sprite.name)
            .ToList();
        Repaint();
    }

    private void CreateObject()
    {
        if (includeSprite && selectedSprite == null)
        {
            EditorUtility.DisplayDialog("Factory", "Select a sprite or disable Include sprite.", "OK");
            return;
        }

        FactoryTypeOptions typeOptions = GetTypeOptions();
        if (typeOptions != null && !typeOptions.Validate(out string validationError))
        {
            EditorUtility.DisplayDialog("Factory", validationError, "OK");
            return;
        }

        var config = new FactoryConfiguration
        {
            typeName = selectedType,
            objectName = objectName,
            prefabName = prefabName,
            sprite = includeSprite ? selectedSprite : null,
            worldPosition = worldPosition,
            worldZ = worldZ,
            worldWidth = worldWidth,
            worldHeight = worldHeight,
            spriteTint = spriteTint,
            sortingLayerName = sortingLayerName,
            sortingOrder = sortingOrder,
            addCollider = selectedType == FactoryConfiguration.TypeInteractive.Trigger || addCollider,
            colliderIsTrigger = selectedType == FactoryConfiguration.TypeInteractive.Trigger || colliderIsTrigger,
            addRigidbody = addRigidbody,
            bodyType = bodyType,
            addOutliner = addOutliner,
            outlineColor = outlineColor,
            interactionOutlineColor = interactionOutlineColor,
            outlineScale = outlineScale,
            outlineinteractionDistane = outlineinteractionDistane,
            visualOffset = visualOffset,
            interactiveDescription = interactiveDescription,
            includeInNodeGraph = includeInNodeGraph,
            componentType = typeOptions != null ? typeOptions.ComponentType : null,
            savePrefab = savePrefab,
            prefabFolder = prefabFolder
        };

        GameObject created = GameObjectFactory.Create(config, typeOptions);
        if (created != null)
        {
            Selection.activeGameObject = created;
            EditorGUIUtility.PingObject(created);
        }
    }

    private FactoryTypeOptions GetTypeOptions()
    {
        switch (selectedType)
        {
            case FactoryConfiguration.TypeInteractive.NPC:
                return npcOptions;

            case FactoryConfiguration.TypeInteractive.InteractObject:
                return interactObjectOptions;
            
            case FactoryConfiguration.TypeInteractive.collectableObject:
                return collectableObjectOptions;

            case FactoryConfiguration.TypeInteractive.Buton:
                return emptyOptions;

            case FactoryConfiguration.TypeInteractive.Trigger:
                return triggerOptions;

            case FactoryConfiguration.TypeInteractive.Decoration:
                return decorationOptions;

            case FactoryConfiguration.TypeInteractive.Door:
                return doorOptions;

            default:
                return null;
        }
    }
}
