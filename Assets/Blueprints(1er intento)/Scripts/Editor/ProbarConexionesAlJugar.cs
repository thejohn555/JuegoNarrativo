using System.IO;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;
using Blueprints.Scripts;
using Blueprints.Scripts.BaseClases;

[InitializeOnLoad]
public static class GuardarConexionesAlJugar
{
    static GuardarConexionesAlJugar()
    {
        EditorApplication.playModeStateChanged += AlCambiarPlayMode;
    }

    private static void AlCambiarPlayMode(PlayModeStateChange estado)
    {
        if (estado != PlayModeStateChange.EnteredPlayMode) return;

        foreach (string rutaSucia in Directory.GetFiles("Assets", "*." + PuzzleGraphTest.AssetExtension, SearchOption.AllDirectories))
        {
            string ruta = rutaSucia.Replace('\\', '/');

            var graph = GraphDatabase.LoadGraph<PuzzleGraphTest>(ruta);
            if (graph == null) continue;

            foreach (var node in graph.GetNodes())
            {
                if (node is NodeData nodata)
                {
                    nodata.CollectConexions();
                }
            }
        }

        Debug.Log("Conexiones guardadas al empezar el juego");
    }
}