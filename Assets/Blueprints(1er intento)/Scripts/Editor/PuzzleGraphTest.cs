using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Blueprints.Scripts
{
    [Graph(AssetExtension)]
    [Serializable]
    public class PuzzleGraphTest:Graph
    {
        public const string AssetExtension = "puzzlegraph";

        [MenuItem("Assets/Create/Blueprints/Puzzle Graph")]
        private static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<PuzzleGraphTest>("PuzzleGraph");
        }
    }
}