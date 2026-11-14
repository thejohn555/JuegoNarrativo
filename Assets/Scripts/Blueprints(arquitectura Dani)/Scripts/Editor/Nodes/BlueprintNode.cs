using System;
using Unity.GraphToolkit.Editor;
namespace Blueprints_arquitectura_Dani_.Editor.Nodes

{
    [Serializable]
    public abstract class BlueprintNode:Node
    {
        public const string ExecutionPortDefaultName = "Execution Port";
    }
}