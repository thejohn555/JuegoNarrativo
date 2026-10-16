using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Blueprints.Scripts.BaseClases
{
    public abstract class NodeData:Node
    {
        [SerializeField] private string id;      // id único persistente
        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                    id = Guid.NewGuid().ToString();
                return id;
            }
        }
        public abstract NodeType Type { get; }
        public abstract List<PortData> Ports { get; }
        
        
        public NodeData SearchNodeData(INode node)
        {
            var mynode = node as NodeData;
            if (node == null)
            {
                Debug.Log("Null node");
                return null;
            }
            else
            {
             return mynode;
            }
        }
        public virtual void CollectConexions()
        {}
    }
    
}