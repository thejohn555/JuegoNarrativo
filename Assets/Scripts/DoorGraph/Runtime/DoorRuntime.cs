using System.Collections.Generic;

namespace DoorGraph.Editor
{
    public class DoorRuntime
    {
        readonly DoorGraphAsset asset;
        readonly int[] activeOutput;
        readonly Dictionary<string, int> indexById = new();

        public DoorRuntime(DoorGraphAsset asset)
        {
            this.asset = asset;
            activeOutput = new int[asset.nodes.Count];

            for (int i = 0; i < asset.nodes.Count; i++)
                indexById[asset.nodes[i].id] = i;
        }

        public string IndexOf(int index) => asset.nodes[index].id;
        
        public bool SetActiveOutput(string id, int output)
        {
            if (!indexById.TryGetValue(id, out int node)) return false;
            if (output < 0 || output >= asset.nodes[node].outputs.Count) return false;

            activeOutput[node] = output;
            return true;
        }
        public int Resolve(string id)
        {
            if (!indexById.TryGetValue(id, out int node)) return -1;

            var outputs = asset.nodes[node].outputs;
            int n = activeOutput[node];

            if (n < 0 || n >= outputs.Count) return -1;
            return outputs[n];
        }
        public bool TryGetActiveOutput(string id, out int output)
        {
            output = -1;
            if (!indexById.TryGetValue(id, out int node)) return false;
            output = activeOutput[node];
            return true;
        }
    }
}