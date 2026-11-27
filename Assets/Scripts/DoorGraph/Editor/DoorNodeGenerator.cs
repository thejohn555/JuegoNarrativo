using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DoorGraph.Editor
{
    public static class DoorNodeGenerator
    {
        const string OutputFolder = "Assets/Scripts/DoorGraph/Editor/Node/Generated";

        public static void GenerateAll()
        {
            var manager = Object.FindFirstObjectByType<DoorManager>();
            if (manager == null) return;

            string scene = manager.gameObject.scene.name;
            var names = new List<string>();
            foreach (var go in manager.GetDoors())
                if (go != null)
                    names.Add(go.name);

            Generate(scene, names);
        }

        static void Generate(string scene, List<string> names)
        {
            string sceneId = ToIdentifier(scene);
            string path = $"{OutputFolder}/DoorNodes_{sceneId}.cs";

            var sb = new StringBuilder();
            sb.AppendLine("//(auto generado) No editar a mano.");
            sb.AppendLine("using System;");
            sb.AppendLine("using Unity.GraphToolkit.Editor;");
            sb.AppendLine("using DoorGraph.Editor.Node;");
            sb.AppendLine("namespace Doors.Editor.Generated");
            sb.AppendLine("{");

            var used = new HashSet<string>();
            foreach (string name in names)
            {
                if (string.IsNullOrWhiteSpace(name)) continue;

                string id = ToIdentifier(name);
                if (!used.Add(id))
                {
                    Debug.LogWarning($"[Doors] Nombre repetido: '{name}', se ignora");
                    continue;
                }

                sb.AppendLine("    [Serializable]");
                sb.AppendLine("    [UseWithGraph(typeof(DoorGraph.Editor.DoorGraph))]");
                sb.AppendLine($"    [Node(categoryPath:\"Doors/{scene}\")]");
                sb.AppendLine($"    public class Door_{sceneId}_{id} : DoorBaseNode");
                sb.AppendLine($"    {{ public override string Key => \"{name}\"; }}");
            }

            sb.AppendLine("}");

            string content = sb.ToString();
            if (File.Exists(path) && File.ReadAllText(path) == content) return; // evita bucles de recompilación

            Directory.CreateDirectory(OutputFolder);
            File.WriteAllText(path, content);
            AssetDatabase.Refresh();
        }

        static string ToIdentifier(string name)
        {
            var sb = new StringBuilder();
            foreach (char c in name)
                sb.Append(char.IsLetterOrDigit(c) ? c : '_');
            return sb.ToString();
        }
    }


[InitializeOnLoad]
    static class DoorNodeGeneratorHook
    {
        static DoorNodeGeneratorHook()
        {
            DoorManager.DoorsChanged += DoorNodeGenerator.GenerateAll;
        }
    }
    }
