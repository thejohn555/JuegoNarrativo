using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;
using System.Collections.Generic;
using Blueprints_arquitectura_Dani_.Editor;
using Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime.InteractNode;
using Blueprints_arquitectura_Dani_.Runtime.LevelNode;
using Blueprints_arquitectura_Dani_.Runtime.TriggerNode;
using Unity.GraphToolkit.Editor;
using UnityEngine;

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
        // switch (nodeModel)
        // {
        //     case IfNode ifNode:
        //
        //         bool condition = false;
        //         ifNode.GetNodeOptionByName("Condition")?.TryGetValue(out condition);
        //         list.Add(new IfRunTimeNode
        //         {
        //             Condition = condition
        //         });
        //         return true;
        // }
        return false;
    }

    private static bool FindSequenceNodes(INode nodeModel, List<RuntimeNode> list)
    {
        switch (nodeModel)
        {
            case StartNode startNode:
                list.Add(new StartRuntimeNode());
                return true;
            case ConditionNode conditionNode:
                list.Add(new ConditionRuntimeNode());
                return true;
            case OnTriggerEnterNode triggerNode:
                list.Add(new TriggerEnterRuntimeNode(triggerNode.Key));
                return true;
            case OnInteractNode onInteractNode:
                list.Add(new InteractRuntimeNode(onInteractNode.Key));
                return true;
                
        }
        return false;
    }
}