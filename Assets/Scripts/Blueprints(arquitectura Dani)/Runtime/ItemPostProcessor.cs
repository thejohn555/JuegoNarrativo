using UnityEditor;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class ItemPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] moved,
            string[] movedFrom)
        {
            foreach (var path in importedAssets)
            {
                if (!path.EndsWith(".prefab"))
                {
                    continue;
                }
                else
                {
                    EditorApplication.delayCall += () => EnsureItemId(path);
                }
            }
        }

        private static void EnsureItemId(string path)
        {
            
        }
    }
}