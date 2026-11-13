using UnityEditor;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public static class RepairItems
    {
        [MenuItem("Blueprints/Reparar IDs de items")]
        private static void Repair()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            int fixedCount = 0;
            int okCount = 0;

            try
            {
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                    EditorUtility.DisplayProgressBar("Reparando IDs de items", path, (float)i / guids.Length);

                    if (FixPrefab(path)) fixedCount++;
                    else okCount++;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[RepairItemIds] Reparados: {fixedCount} | Ya estaban bien o no son items: {okCount}");
        }

        // Devuelve true si ha modificado el prefab
        private static bool FixPrefab(string path)
        {
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                // Solo nos interesan los items
                if (!root.TryGetComponent<Interactives.CollectableObject>(out _)) return false;

                string correctGuid = AssetDatabase.AssetPathToGUID(path);

                if (!root.TryGetComponent<BlueprintItemId>(out var id))
                    id = root.AddComponent<BlueprintItemId>();

                if (id.Guid == correctGuid) return false;   // ya está bien

                Debug.Log($"[RepairItemIds] {path}: '{id.Guid}' -> '{correctGuid}'");
                id.SetGuid(correctGuid);
                PrefabUtility.SaveAsPrefabAsset(root, path);
                return true;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}