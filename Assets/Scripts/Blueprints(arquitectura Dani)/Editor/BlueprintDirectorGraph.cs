using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;


namespace Blueprints_arquitectura_Dani_.Editor
{
    [Serializable]
    [Graph(AssetExtension)]
    public class BlueprintDirectorGraph:Graph
    {
        public const string AssetExtension = "Blueprints";
        [MenuItem("Assets/Create/Blueprints/Blueprints Graph ")]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<BlueprintDirectorGraph>("Blueprint Graph");
        }
    }
}