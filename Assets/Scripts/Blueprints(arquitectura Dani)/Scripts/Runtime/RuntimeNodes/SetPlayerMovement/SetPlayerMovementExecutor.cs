using Blueprints_arquitectura_Dani_.Runtime;
using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.SetPlayerMovement
{
    public class SetPlayerMovementExecutor:BlueprintNodeExecutor<SetPlayerMovementRuntime>
    {
        protected override int OnExecute(SetPlayerMovementRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            BlueprintsEventManager.Instance.SetMovement(node.active);
            return 0;
        }
    }
}