using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Blueprints.Scripts.BaseClases
{
    public class Conexions:MonoBehaviour
    {
        public static Conexions Instance { get;private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        
        private List<ConexionData> conexions = new List<ConexionData>();

        public void AddConexion(NodeData originNode, PortData originPort, NodeData destinationData,
            PortData destinationPort)
        {
            conexions.Add(new ConexionData(originNode, originPort, destinationData, destinationPort));
        }
        
        
    }
}