using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FactoryWindow : EditorWindow
{
    public FactoryConfiguration.TypeInteractive selectedType;

    private string search = "";
    private Vector2 spriteScroll;
    private Vector2 windowScroll;
    private Sprite selectedSprite;
    private bool includeSprite = true;
    private float worldWidth = 1f;
    private float worldHeight = 1f;
    private Vector2 worldPosition = Vector2.zero;
    private Color spriteTint = Color.red;

    private readonly NpcFactoryTypeOptions npcOptions = new NpcFactoryTypeOptions();
    private readonly InteractObjectFactoryTypeOptions interactObjectOptions =
        new InteractObjectFactoryTypeOptions();
    private readonly EmptyFactoryTypeOptions emptyOptions = new EmptyFactoryTypeOptions();

    private bool addCollider = true;
    private bool colliderIsTrigger;
    private bool addRigidbody;
    private RigidbodyType2D bodyType = RigidbodyType2D.Static;
    private bool addOutliner;
    private Color outlineColor = Color.white;
    private float outlineScale = 1.1f;

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
            "Crea objetos 2D en la escena con sprite, tamano en unidades del mundo y componentes configurables.",
            MessageType.Info);

        EditorGUILayout.Space(6);
        DrawTypeSection();
        EditorGUILayout.Space(8);
        DrawTypeOptions();
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Sprite", EditorStyles.boldLabel);
        includeSprite = EditorGUILayout.Toggle("Incluir sprite", includeSprite);
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
            if (GUILayout.Button("Crear objeto en escena", GUILayout.Height(34)))
                CreateObject();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawTypeSection()
    {
        EditorGUILayout.LabelField("Tipo de objeto", EditorStyles.boldLabel);
        selectedType = (FactoryConfiguration.TypeInteractive)EditorGUILayout.EnumPopup("Tipo", selectedType);
    }

    private void DrawTypeOptions()
    {
        FactoryTypeOptions typeOptions = GetTypeOptions();
        if (typeOptions != null)
            typeOptions.DrawOptions();
    }

    private void DrawSpriteBrowser()
    {
        EditorGUILayout.BeginHorizontal();
        string newSearch = EditorGUILayout.TextField("Buscar", search);
        if (newSearch != search)
        {
            search = newSearch;
            RefreshSprites();
        }

        if (GUILayout.Button("Actualizar", GUILayout.Width(78)))
            RefreshSprites();
        EditorGUILayout.EndHorizontal();

        selectedSprite = (Sprite)EditorGUILayout.ObjectField(
            "Sprite elegido", selectedSprite, typeof(Sprite), false);

        if (selectedSprite != null)
        {
            GUILayout.Label(AssetPreview.GetAssetPreview(selectedSprite) ??
                            AssetPreview.GetMiniThumbnail(selectedSprite),
                GUILayout.Width(100), GUILayout.Height(100));
            EditorGUILayout.LabelField("Nombre", selectedSprite.name);
            EditorGUILayout.LabelField("Tamano original",
                $"{selectedSprite.rect.width:0} x {selectedSprite.rect.height:0} px");
        }

        EditorGUILayout.LabelField($"Sprites encontrados: {sprites.Count}", EditorStyles.miniLabel);
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
        EditorGUILayout.LabelField("Tamano y posicion", EditorStyles.boldLabel);
        worldWidth = Mathf.Max(0.01f, EditorGUILayout.FloatField("Ancho (unidades)", worldWidth));
        worldHeight = Mathf.Max(0.01f, EditorGUILayout.FloatField("Alto (unidades)", worldHeight));
        worldPosition = EditorGUILayout.Vector2Field("Posicion X / Y", worldPosition);

        if (includeSprite)
        {
            spriteTint = EditorGUILayout.ColorField("Color del sprite", spriteTint);
            if (selectedSprite != null)
            {
                Vector2 size = GetSpriteWorldSize(selectedSprite);
                EditorGUILayout.LabelField("Tamano base estimado",
                    $"{size.x:0.###} x {size.y:0.###} unidades");
            }
        }
    }

    private void DrawComponentsSection()
    {
        EditorGUILayout.LabelField("Componentes", EditorStyles.boldLabel);
        addCollider = EditorGUILayout.Toggle("Anadir Collider2D", addCollider);
        if (addCollider)
            colliderIsTrigger = EditorGUILayout.Toggle("Es Trigger", colliderIsTrigger);

        addRigidbody = EditorGUILayout.Toggle("Anadir Rigidbody2D", addRigidbody);
        if (addRigidbody)
            bodyType = (RigidbodyType2D)EditorGUILayout.EnumPopup("Tipo Rigidbody", bodyType);

        addOutliner = EditorGUILayout.Toggle("Configurar Outliner", addOutliner);
        if (addOutliner)
        {
            outlineColor = EditorGUILayout.ColorField("Color del contorno", outlineColor);
            outlineScale = EditorGUILayout.Slider("Escala del contorno", outlineScale, 1f, 1.5f);
        }
    }

    private void DrawPrefabSection()
    {
        EditorGUILayout.LabelField("Prefab opcional", EditorStyles.boldLabel);
        savePrefab = EditorGUILayout.Toggle("Guardar como prefab", savePrefab);
        using (new EditorGUI.DisabledScope(!savePrefab))
        {
            prefabFolder = EditorGUILayout.TextField("Carpeta", prefabFolder);
            EditorGUILayout.HelpBox(
                "La carpeta se creara si no existe. La ruta debe estar dentro de Assets.",
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
            EditorUtility.DisplayDialog("Factory", "Selecciona un sprite o desactiva Incluir sprite.", "Aceptar");
            return;
        }

        FactoryTypeOptions typeOptions = GetTypeOptions();
        if (typeOptions != null && !typeOptions.Validate(out string validationError))
        {
            EditorUtility.DisplayDialog("Factory", validationError, "Aceptar");
            return;
        }

        var config = new FactoryConfiguration
        {
            typeName = selectedType,
            sprite = includeSprite ? selectedSprite : null,
            worldPosition = worldPosition,
            worldWidth = worldWidth,
            worldHeight = worldHeight,
            spriteTint = spriteTint,
            addCollider = addCollider,
            colliderIsTrigger = colliderIsTrigger,
            addRigidbody = addRigidbody,
            bodyType = bodyType,
            addOutliner = addOutliner,
            outlineColor = outlineColor,
            outlineScale = outlineScale,
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

            case FactoryConfiguration.TypeInteractive.Buton:
                return emptyOptions;

            default:
                return null;
        }
    }
}
