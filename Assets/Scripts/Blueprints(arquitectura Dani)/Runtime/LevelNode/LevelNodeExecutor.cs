using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.LevelNode
{
    public class LevelNodeExecutor:BlueprintNodeExecutor<LevelNodeRuntime>
    {
        protected override int OnExecute(LevelNodeRuntime node, BlueprintDirector ctx)
        {
            Debug.Log(node.level);
            return -1;
        }
    }
}