using Assets.Scripts.Camera;
using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.StartDialogue
{
    public class StartDialogueExecutor : BlueprintNodeExecutor<StartDialogueRuntime>
    {
        protected override int OnExecute(StartDialogueRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            Debug.Log("[StartDialogueExecutor] Executando con el objeto");
            Debug.Log("[StartDialogueExecutor] Nombre: " + node.goName);
            if (string.IsNullOrEmpty(node.goName)) return -1;

            GameObject npc = BlueprintDictionaryManager.Instance.ResolveReference(node.goName);
            if (npc == null)
            {
                Debug.LogWarning($"[StartDialogue] No encuentro el objeto '{node.goName}'");
                return -1;
            }

            BlueprintsEventManager.Instance.StartNpcDialogue(npc);
            return 0;

        }
    }
}
   