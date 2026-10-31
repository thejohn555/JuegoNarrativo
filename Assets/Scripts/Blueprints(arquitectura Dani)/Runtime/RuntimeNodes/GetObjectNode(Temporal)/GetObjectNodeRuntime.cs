using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.GetObjectNode_Temporal_
{
    public class GetObjectNodeRuntime:RuntimeNode
    {
        public string goName;

        public GetObjectNodeRuntime(string goName)
        {
            this.goName = goName;
        }
    }
}