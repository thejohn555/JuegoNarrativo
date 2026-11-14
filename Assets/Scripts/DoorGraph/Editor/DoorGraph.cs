using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace DoorGraph.Editor
{
    [Serializable]
    [Graph(AssetExtension)]
    public class DoorGraph:Graph
    {
        public const string AssetExtension = "Door";
        [MenuItem("Assets/Create/Door/Door Graph ")]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DoorGraph>("Door Graph ");
        }
    }
}