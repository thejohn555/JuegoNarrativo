using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.LevelNode
{
    public class LevelNodeExecutor:IBlueprintNodeExecutor<LevelNodeRuntime>
    {
        public bool Execute(LevelNodeRuntime node, BlueprintDirector ctx)
        {
            Debug.Log(node.level);
            return true;
        }
    }
}