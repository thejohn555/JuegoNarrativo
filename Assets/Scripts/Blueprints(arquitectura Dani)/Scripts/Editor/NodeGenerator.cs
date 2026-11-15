using System.Collections.Generic;
using System.IO;
using System.Text;
using Blueprints_arquitectura_Dani_.Runtime;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEditor;
using UnityEngine;


//-------------IMPORTANTE--------------------
//NO SE GENERARAN NODOS CON EL MISMO NOMBRE


//Para crear un nodo base de inicio que requiera un objeto de la esecena
//Seguir esta base
// [Serializable];
// public class {prefix}{id} : {baseClass}
// public override string Key => \"{name}\";
namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor
{

    
    public class NodeGenerator
    { 
        public static NodeGenerator Instance { get; } = new NodeGenerator();
      private const string outputFolder = "Assets/Scripts/Blueprints(arquitectura Dani)/Scripts/Editor/Nodes/Generated";
      
      static void GenerateFromScene(NodeTypes type)
      {
          var dictionaryManager = Object.FindFirstObjectByType<BlueprintDictionaryManager>();
          if (dictionaryManager == null) return;   // escena sin manager (menú, etc.): no es un error
          string scene = dictionaryManager.gameObject.scene.name;

          var names = new List<string>();
          foreach (var go in dictionaryManager.GetObjects(type))
              if (go != null) names.Add(go.name);

          Generate(type, scene, names);
      }

      private static void Generate(NodeTypes type,string scene,List<string> names)
      {
          var (prefix, baseClass, file, category) = NodeGenTemplate.Map[type];
          string sceneId = ToIdentifier(scene);
          string path = $"{outputFolder}/{Path.GetFileNameWithoutExtension(file)}_{sceneId}.cs";
          var sb = new StringBuilder();
          sb.AppendLine("//(auto generado) No editar a mano.");
          sb.AppendLine("using System;");
          sb.AppendLine("using Unity.GraphToolkit.Editor;");
          sb.AppendLine("namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes.Generated");
          sb.AppendLine("{");
          
          var used = new HashSet<string>();
          foreach (string name in names)
          {
              if (string.IsNullOrWhiteSpace(name))
              {
                  continue;
              }
              else
              {
                  string id = ToIdentifier(name);

                  if (used.Contains(id))
                  {
                      continue;
                  }

                  sb.AppendLine("    [Serializable]");
                  sb.AppendLine($"    [Node(categoryPath:\"Nodes/{scene}/{category}\")]");
                  sb.AppendLine($"    public class {prefix}{sceneId}_{id} : {baseClass}");
                  sb.AppendLine($"    {{ public override string Key => \"{name}\"; }}");
                  used.Add(id);
              }
          }
              sb.AppendLine("}");
              string content = sb.ToString();
              if (File.Exists(path) && File.ReadAllText(path) == content)
              {
                  //Debug.Log("[TriggerNodes] Sin cambios");
                  return;
              }

              Directory.CreateDirectory(outputFolder);
              File.WriteAllText(path, content);
              AssetDatabase.Refresh();
              Debug.Log($"[NodeGenerator] {type}: {used.Count} clases");
          
      }
      
      static string ToIdentifier(string name)
      {
          var sb = new StringBuilder();
          foreach (char c in name)
          {
              if (char.IsLetterOrDigit(c))
              {
                  sb.Append(c);
              }
              else
              {
                  sb.Append('_');
              }
          }
          return sb.ToString();
      }
      public static void GenerateAll()
      {
          foreach (var type in NodeGenTemplate.Map.Keys)
              GenerateFromScene(type);
      }
    }
}