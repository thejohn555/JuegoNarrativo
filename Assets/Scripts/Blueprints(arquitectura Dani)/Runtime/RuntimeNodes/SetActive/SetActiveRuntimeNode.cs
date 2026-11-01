namespace Blueprints_arquitectura_Dani_.Runtime.RuntimeNodes.SetActive
{
    public class SetActiveRuntimeNode:RuntimeNode
    {
        public string goName;
        public bool active;

        public SetActiveRuntimeNode(string goName, bool active)
        {
            this.goName = goName;
            this.active = active;
        }
    }
}