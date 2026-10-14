using System.Xml;
using Unity.GraphToolkit.Editor;

namespace Blueprints.Scripts.BaseClases
{
    public class PortData
    {
        public int Id{ get; }
        public PortType Type { get; }
        public PortDirection Direction { get; }

        public  PortData(int id,PortType type,PortDirection direction)
        {
            this.Id = id;
            this.Type = type;
            this.Direction = direction;
        }

        public bool IsConected(Node node)
        {
            IPort port;
            if (Direction == PortDirection.Input)
            {
                port = node.GetInputPort(Id);
            }
            else
            {
                port = node.GetOutputPort(Id);
            }
            return port != null && port.IsConnected;
        }
        public PortData SearchPortData(NodeData nodo, IPort puerto)
        {
            foreach (var port in nodo.Ports)
            {
                if (port.Direction != PortDirection.Input) continue;
                if (nodo.GetInputPort(port.Id) == puerto) return port;
            }
            return null;
        }
    }
}