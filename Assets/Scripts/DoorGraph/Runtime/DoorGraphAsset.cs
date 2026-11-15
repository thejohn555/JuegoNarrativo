using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoorGraph.Editor
{
    public class DoorGraphAsset : ScriptableObject
    {
        public List<DoorNodeData> nodes = new();
    }
    [Serializable]
    public class DoorNodeData
    {
        public string id;                  
        public List<int> outputs = new();
        
    }
    
}