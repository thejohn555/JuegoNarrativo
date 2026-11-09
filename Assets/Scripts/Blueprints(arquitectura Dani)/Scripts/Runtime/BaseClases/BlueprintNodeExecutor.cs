using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.BaseClases
{
    public abstract class BlueprintNodeExecutor<T> :IBlueprintNodeExecutor where T : RuntimeNode
    {
        public int Execute(RuntimeNode node, BlueprintDirector ctx,GameObject obj) => OnExecute((T)node, ctx,obj);

        protected abstract int OnExecute(T node, BlueprintDirector ctx,GameObject obj);
    }
}