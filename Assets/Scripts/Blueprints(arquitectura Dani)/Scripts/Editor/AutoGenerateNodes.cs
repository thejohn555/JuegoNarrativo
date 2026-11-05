using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEditor;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor
{
    [InitializeOnLoad]
    public class AutoGenerateNodes
    {
        static AutoGenerateNodes()
        {
            BlueprintDictionaryManager.NodesChanged -= OnChanged;
            BlueprintDictionaryManager.NodesChanged += OnChanged;
        }

        static void OnChanged()
        {
            EditorApplication.delayCall -= NodeGenerator.GenerateAll;
            EditorApplication.delayCall += NodeGenerator.GenerateAll;
        }
    }
}