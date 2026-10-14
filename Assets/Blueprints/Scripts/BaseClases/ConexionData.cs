namespace Blueprints.Scripts.BaseClases
{
    public class ConexionData
    {
        private NodeData originNode;
        private PortData originPort;
        private  NodeData destinationNode;
        private PortData destinationPort;

        public ConexionData(NodeData originNode, PortData originPort, NodeData destinationNode,PortData destinationPort)
        {
            this.originNode = originNode;
            this.originPort = originPort;
            this.destinationNode = destinationNode;
            this.destinationPort = destinationPort;
        }
    }
}