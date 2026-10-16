using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

public static class NodeTranslator
{
    public static void AnalizeNodes(INode node, List<RuntimeNode> list)
    {
        if (FindConditionNodes(node, list))
        {
            return;
        }
        FindSequenceNodes(node, list);
    }

    private static bool FindConditionNodes(INode nodeModel, List<RuntimeNode> list)
    {
        switch (nodeModel)
        {
            //case IfNode ifNode:

            //    bool condition = false;
            //    ifNode.GetNodeOptionByName("Condition")?.TryGetValue(out condition);
            //    list.Add(new IfRunTimeNode
            //    {
            //        Condition = condition
            //    });
            //    return true;
        }
        return false;
    }

    private static bool FindSequenceNodes(INode nodeModel, List<RuntimeNode> list)
    {
        switch (nodeModel)
        {
            case StartNode startNode:
                list.Add(new StartRuntimeNode());
                return true;
        }
        return false;
    }
}