using UnityEditor;
using UnityEditor.SceneManagement;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor
{
    [InitializeOnLoad]
    public class GenerateNodesOnSceneOpen
    {
        static  GenerateNodesOnSceneOpen()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorApplication.delayCall += Generate;
        }

        private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            EditorApplication.delayCall -= Generate;
            EditorApplication.delayCall += Generate;
        }
        static void Generate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            NodeGenerator.GenerateAll();
        }
    }
}