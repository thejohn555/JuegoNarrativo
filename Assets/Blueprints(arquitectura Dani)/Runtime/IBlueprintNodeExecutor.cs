

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public interface IBlueprintNodeExecutor<in TNode> where TNode : RuntimeNode
    {
        bool Execute(TNode node,BlueprintDirector ctx);
    }
}