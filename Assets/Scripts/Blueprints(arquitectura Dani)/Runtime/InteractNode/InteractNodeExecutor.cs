using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.InteractNode
{
    public class InteractNodeExecutor:BlueprintNodeExecutor<InteractRuntimeNode>
    {
        

        protected override int OnExecute(InteractRuntimeNode node, BlueprintDirector ctx, GameObject obj)
        {
            Debug.Log("InteractNodeExecutor");
            return 1;
        }
    }
}