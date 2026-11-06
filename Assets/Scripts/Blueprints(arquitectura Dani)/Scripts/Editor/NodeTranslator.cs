using Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime;
using System.Collections.Generic;
using Blueprints_arquitectura_Dani_.Editor;
using Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Editor.Blueprints_arquitectura_Dani_.Scripts.Editor.Nodes;
using Blueprints_arquitectura_Dani_.Runtime.InteractNode;
using Blueprints_arquitectura_Dani_.Runtime.LevelNode;
using Blueprints_arquitectura_Dani_.Runtime.Managers;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.AddItemToInventory;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.DelayNode;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.MoveToNode;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.OnInteractNpcNode;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.SetActive;
using Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.StartDialogue;
using Blueprints_arquitectura_Dani_.Runtime.TriggerNode;
using Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.ChangeNpcState;
using Unity.GraphToolkit.Editor;
using UnityEditor;
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
            case OnTriggerEnterNode triggerNode:
                list.Add(new TriggerEnterRuntimeNode(triggerNode.Key));
                return true;
            case OnInteractNode onInteractNode:
            {
                GameObject prefab = null;
                onInteractNode.GetNodeOptionByName(OnInteractNode.RequiredObjectOption)?.TryGetValue(out prefab);

                string requiredGuid = null;

                if (prefab != null)
                {
                    string path = AssetDatabase.GetAssetPath(prefab);
                    requiredGuid = AssetDatabase.AssetPathToGUID(path);
                }
                bool destroy = false;
                onInteractNode.GetNodeOptionByName(OnInteractNode.RequiredBoolOption);
                list.Add(new InteractRuntimeNode(onInteractNode.Key, requiredGuid,destroy));
                return true;
            }
            case OnInteractNpcNode onInteractNpcNode:
            {
                GameObject prefab = null;
                onInteractNpcNode.GetNodeOptionByName(OnInteractNpcNode.RequiredObjectOption)?.TryGetValue(out prefab);

                string requiredGuid = null;

                if (prefab != null)
                {
                    string path = AssetDatabase.GetAssetPath(prefab);
                    requiredGuid = AssetDatabase.AssetPathToGUID(path);
                }
                bool destroy = false;
                onInteractNpcNode.GetNodeOptionByName(OnInteractNpcNode.RequiredBoolOption);
                list.Add(new OnInteractNpcRuntime(onInteractNpcNode.Key, requiredGuid,destroy));
                return true;
            }  
            case SetActiveNode setActiveNode:
            {
                var port = setActiveNode.GetInputPortByName(SetActiveNode.ObjectPort);
                string key = null;
                if (port != null && port.IsConnected &&
                    port.FirstConnectedPort.GetNode() is ReferenceNode refNode)
                {
                    key = refNode.Key;
                }

                bool active;
                setActiveNode.GetNodeOptionByName(SetActiveNode.BoolOption).TryGetValue(out active);
                list.Add(new SetActiveRuntimeNode(key, active));
                return true;
            }

            case DelayNode delayNode:
                float delay = 0;
                delayNode.GetNodeOptionByName(DelayNode.TimeOption)?.TryGetValue(out delay);
                list.Add(new DelayNodeRuntime(delay));
                return true;
            case MoveToNode moveToNode:
            {
                var port = moveToNode.GetInputPortByName(MoveToNode.ObjectPort);
                string key = null;
                if (port != null && port.IsConnected &&
                    port.FirstConnectedPort.GetNode() is ReferenceNode refNode)
                {
                    key = refNode.Key;
                }

                float time;
                moveToNode.GetNodeOptionByName(MoveToNode.TimeOption).TryGetValue(out time);
                Vector2 pos;
                moveToNode.GetNodeOptionByName(MoveToNode.PositionOption).TryGetValue(out pos);
                list.Add(new MoveToRuntimeNode(key, pos,time));
                return true; 
            }
            case StartDialogueNode dialogueRuntime:
            {
                var port = dialogueRuntime.GetInputPortByName(StartDialogueNode.ObjectPort);
                string key = null;
                if (port != null && port.IsConnected &&
                    port.FirstConnectedPort.GetNode() is ReferenceNode refNode)
                {
                    key = refNode.Key;
                }
                list.Add(new StartDialogueRuntime(key));
                return true;
            }
            case AddItemToInventoryNode addNode:
            {
                GameObject prefab = null;
                addNode.GetNodeOptionByName(AddItemToInventoryNode.RequiredObjectOption)?.TryGetValue(out prefab);
                string requiredGuid = null;
                if (prefab == null)
                {
                    Debug.Log("Prefab not found");
                }
                SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
                Sprite itemSprite = spriteRenderer != null ? spriteRenderer.sprite : null;
                string path = AssetDatabase.GetAssetPath(prefab);
                requiredGuid = AssetDatabase.AssetPathToGUID(path);
                list.Add(new AddItemToInventoryRuntime(requiredGuid,prefab.name,itemSprite));
                return true;
            }
            case ChangeNpcState changeNpcState:
            {
                string id;
                changeNpcState.GetNodeOptionByName(ChangeNpcState.NpcId).TryGetValue(out id);
                string state;
                changeNpcState.GetNodeOptionByName(ChangeNpcState.NpcStateName).TryGetValue(out state);
                list.Add(new ChangeNpcStateRuntime(state, id));
                return true;

            }
                
                
        }
        return false;
    }
}