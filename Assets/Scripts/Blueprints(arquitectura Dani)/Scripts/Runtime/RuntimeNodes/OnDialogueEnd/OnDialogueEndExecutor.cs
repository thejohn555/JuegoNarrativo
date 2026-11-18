using Blueprints_arquitectura_Dani_.Runtime;
using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.OnDialogueEnd
{
    public class OnDialogueEndExecutor:BlueprintNodeExecutor<OnDialogueEndRuntime>
    {
        protected override int OnExecute(OnDialogueEndRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            GameObject go = BlueprintDictionaryManager.Instance.ResolveReference(node.goName);

            if (go != obj )
            {
                return -1;
            }
            else if(!string.IsNullOrEmpty(node.stateId))
            {
                if (node.stateId != BlueprintMediator.Instance.LastNpcState)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            else if(string.IsNullOrEmpty(BlueprintMediator.Instance.LastNpcState))
            {
                return 0;
            }
            return -1;
            
        }
    }
}