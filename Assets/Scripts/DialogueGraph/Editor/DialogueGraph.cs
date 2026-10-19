using System;
using Unity.GraphToolkit.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Graph = Unity.GraphToolkit.Editor.Graph;

namespace DialogueGraph.Editor
{
    [Serializable]
    [Graph(AssetExtension)]
    public class DialogueGraph : Graph
    {
        public const string AssetExtension = "dialogue";
        
        [MenuItem("Assets/Create/DialogueGraph", false)]
        public static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>();
        }
    }
}
