using System.Collections.Generic;
using System.IO;
using System.Text;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEditor;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor
{
    public class TriggerNodeGenerator
    {
      public const string outPutPath = "\"Assets/Scripts/Blueprints(arquitectura Dani)/Editor/Nodes/Generated/TriggerNodes.cs\";";

      [MenuItem("Blueprints/Generar nodos de trigger")]
      static void GenerateFromScene()
      {
          var dictionaryManager = Object.FindFirstObjectByType<BlueprintDictionaryManager>();

          if (dictionaryManager == null)
          {
              Debug.LogError("No se ha encontrado BlueprintDictionaryManager");
              return;
          }
          var obj = dictionaryManager.triggerList;
          List<string> names = new List<string>();
          foreach (var gameObj in obj)
          {
              if (gameObj != null)
              {
                  names.Add(gameObj.name);
              }
          }
          Generate(names);
      }

      private static void Generate(List<string> names)
      {
          var sb = new StringBuilder();
          sb.AppendLine("//(auto generado) No editar a mano.");
          sb.AppendLine("using System;");
          sb.AppendLine("namespace Blueprints_arquitectura_Dani_.Editor");
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
                  sb.AppendLine($"    public class OnTriggerEnter_{id} : TriggerEnterNodeBase");
                  sb.AppendLine($"    {{ public override string TriggerKey => \"{name}\"; }}");
                  used.Add(id);
              }
          }
              sb.AppendLine("}");
              string content = sb.ToString();
              if (File.Exists(outPutPath) && File.ReadAllText(outPutPath) == content)
              {
                  Debug.Log("[TriggerNodes] Sin cambios");
                  return;
              }

              Directory.CreateDirectory(Path.GetDirectoryName(outPutPath));
              File.WriteAllText(outPutPath, content);
              AssetDatabase.Refresh();
              Debug.Log($"[TriggerNodes] Generadas {used.Count} clases");
          
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
    }
}