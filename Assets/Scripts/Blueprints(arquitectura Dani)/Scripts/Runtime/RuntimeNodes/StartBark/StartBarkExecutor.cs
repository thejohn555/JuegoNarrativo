using Blueprints_arquitectura_Dani_.Runtime;
using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using Blueprints_arquitectura_Dani_.Runtime.Managers;

using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.StartBark
{
    public class StartBarkExecutor:BlueprintNodeExecutor<StartBarkRuntime>
    {
        protected override int OnExecute(StartBarkRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            Debug.Log("[StartBarkExecutor] OnExecute ejecutado");
            if (string.IsNullOrEmpty(node.NpcName))
            {
                Debug.Log("[StartBarkExecutor] string Null");
                return -1;
            }

            GameObject npc = BlueprintDictionaryManager.Instance.ResolveReference(node.NpcName);
            Debug.Log($"[StartBark] Nombre solicitado: {node.NpcName}");
            Debug.Log($"[StartBark] Objeto resuelto: {(npc != null ? npc.name : "NULL")}");
            if (npc == null)
            {
                Debug.LogWarning($"[StartBark] No encuentro el objeto '{node.NpcName}'");
                return -1;
            }
            BlueprintsEventManager.Instance.StartBark(npc);
            return 0;
        }
    }
}