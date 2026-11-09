using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.MoveToNode
{
    public class MoveToRuntimeNode:RuntimeNode
    {
        public string goName;
        public float time;
        public Vector2 positionToGo;
        public MoveToRuntimeNode(string goName, Vector2 positionToGo,float time)
        {
            this.goName = goName;
            this.positionToGo = positionToGo;
            this.time = time;
        }
    }
}