#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public static class GameObjectFactory
{
    public static GameObject Create(FactoryConfiguration config, FactoryTypeOptions typeOptions)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        string defaultObjectName = config.sprite != null
            ? $"{config.typeName}_{config.sprite.name}"
            : config.typeName.ToString();
        string objectName = string.IsNullOrWhiteSpace(config.objectName)
            ? defaultObjectName
            : config.objectName.Trim();
        GameObject go = new GameObject(objectName);
        Undo.RegisterCreatedObjectUndo(go, "Create " + config.typeName);
        go.transform.position = new Vector3(config.worldPosition.x, config.worldPosition.y, 0f);

        if (config.sprite != null)
            SetWorldSize(go.transform, config.sprite, config.worldWidth, config.worldHeight);
        else
            go.transform.localScale = new Vector3(
                Mathf.Max(0.01f, config.worldWidth),
                Mathf.Max(0.01f, config.worldHeight),
                1f);

        if (config.sprite != null || config.addCollider || config.addRigidbody)
        {
            GameObject visualObject = new GameObject("Visual");
            Undo.RegisterCreatedObjectUndo(visualObject, "Create visual child");
            visualObject.transform.SetParent(go.transform, false);
            if (config.sprite != null)
            {
                Bounds spriteBounds = config.sprite.bounds;
                visualObject.transform.localPosition = new Vector3(
                    -spriteBounds.center.x,
                    -spriteBounds.min.y,
                    0f);
            }
            else
            {
                visualObject.transform.localPosition = Vector3.zero;
            }

            visualObject.transform.localRotation = Quaternion.identity;
            visualObject.transform.localScale = Vector3.one;

            if (config.sprite != null)
            {
                SpriteRenderer renderer = Undo.AddComponent<SpriteRenderer>(visualObject);
                renderer.sprite = config.sprite;
                renderer.color = config.spriteTint;
            }

            if (config.addCollider)
            {
                BoxCollider2D collider = Undo.AddComponent<BoxCollider2D>(visualObject);
                collider.isTrigger = config.colliderIsTrigger;
                if (config.sprite != null)
                {
                    Bounds spriteBounds = config.sprite.bounds;
                    collider.offset = spriteBounds.center;
                    collider.size = spriteBounds.size;
                }
            }

            if (config.addRigidbody)
            {
                Rigidbody2D rb = Undo.AddComponent<Rigidbody2D>(visualObject);
                rb.bodyType = config.bodyType;
            }
        }

        if (config.componentType != null)
        {
            Type componentType = config.componentType;
            if (componentType == typeof(Transform) ||
                !typeof(MonoBehaviour).IsAssignableFrom(componentType) ||
                componentType.IsAbstract ||
                componentType.ContainsGenericParameters)
            {
                Debug.LogWarning("Factory: el script seleccionado no es un MonoBehaviour concreto valido.", go);
            }
            else
            {
                try
                {
                    Undo.AddComponent(go, componentType);
                }
                catch (Exception exception)
                {
                    Debug.LogError(
                        $"Factory: no se pudo anadir el componente {componentType.FullName}. {exception.Message}",
                        go);
                }
            }
        }

        typeOptions?.ConfigureCreatedObject(go);

        // El script interactivo puede exigir Outliner con RequireComponent.
        // Configuramos el componente que Unity ya haya agregado; nunca lo anadimos aqui.
        if (config.addOutliner)
        {
            Outliner outliner = go.GetComponent<Outliner>();
            if (outliner != null)
            {
                outliner.OutlineColor = config.outlineColor;
                outliner.OutlineWidth = config.outlineScale;
                EditorUtility.SetDirty(outliner);
            }
            else
            {
                Debug.LogWarning(
                    "Factory: el objeto no tiene un Outliner que se pueda configurar; no se anadio ninguno.",
                    go);
            }
        }

        go.name = objectName;

        if (config.savePrefab)
            SavePrefab(go, config.prefabFolder, config.prefabName);

        return go;
    }

    private static void SetWorldSize(Transform target, Sprite sprite, float width, float height)
    {
        Vector2 bounds = sprite.bounds.size;
        float baseWidth = Mathf.Max(bounds.x, 0.0001f);
        float baseHeight = Mathf.Max(bounds.y, 0.0001f);

        target.localScale = new Vector3(
            Mathf.Max(0.01f, width) / baseWidth,
            Mathf.Max(0.01f, height) / baseHeight,
            1f);
    }

    private static void SavePrefab(GameObject instance, string folder, string prefabName)
    {
        string normalizedFolder = NormalizeAssetFolder(folder);
        if (normalizedFolder == null)
        {
            EditorUtility.DisplayDialog(
                "Ruta de prefab no valida",
                "La ruta debe estar dentro de Assets, por ejemplo Assets/Prefabs/Generated.",
                "Aceptar");
            return;
        }

        if (!EnsureAssetFolder(normalizedFolder))
        {
            EditorUtility.DisplayDialog(
                "No se pudo crear la carpeta",
                $"No se pudo preparar la carpeta {normalizedFolder}.",
                "Aceptar");
            return;
        }

        string assetName = string.IsNullOrWhiteSpace(prefabName)
            ? instance.name
            : prefabName.Trim();
        string path = AssetDatabase.GenerateUniqueAssetPath(
            $"{normalizedFolder}/{assetName}.prefab");

        try
        {
            GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(
                instance, path, InteractionMode.UserAction);
            if (prefab == null)
            {
                Debug.LogError($"Factory: Unity no pudo guardar el prefab en {path}.", instance);
                return;
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"Factory: prefab guardado en {path}", instance);
        }
        catch (Exception exception)
        {
            Debug.LogError($"Factory: error al guardar el prefab en {path}. {exception.Message}", instance);
        }
    }

    private static string NormalizeAssetFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            return null;

        string normalized = folder.Replace('\\', '/').Trim();
        string[] parts = normalized.Split('/');
        if (parts.Length == 0 || parts[0] != "Assets")
            return null;

        foreach (string part in parts)
        {
            if (string.IsNullOrWhiteSpace(part) || part == "." || part == "..")
                return null;
        }

        return string.Join("/", parts);
    }

    private static bool EnsureAssetFolder(string folder)
    {
        if (AssetDatabase.IsValidFolder(folder))
            return true;

        string[] parts = folder.Split('/');
        string current = "Assets";
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
            {
                string guid = AssetDatabase.CreateFolder(current, parts[i]);
                if (string.IsNullOrEmpty(guid) && !AssetDatabase.IsValidFolder(next))
                    return false;
            }

            current = next;
        }

        return AssetDatabase.IsValidFolder(folder);
    }
}
#endif
