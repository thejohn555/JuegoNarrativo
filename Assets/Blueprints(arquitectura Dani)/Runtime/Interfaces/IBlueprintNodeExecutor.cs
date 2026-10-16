

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public interface IBlueprintNodeExecutor<in TNode>
    {
        bool Execute(TNode node,BlueprintDirector ctx);
    }
}