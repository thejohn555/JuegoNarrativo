using Blueprints_arquitectura_Dani_.Runtime;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.SetInteractable
{
    public class SetInteractableRuntime:RuntimeNode
    {
        public string goNome;
        public bool active;
        public SetInteractableRuntime(string goNome,bool active)
        {
            this.goNome = goNome;
            this.active = active;
        }
    }
}