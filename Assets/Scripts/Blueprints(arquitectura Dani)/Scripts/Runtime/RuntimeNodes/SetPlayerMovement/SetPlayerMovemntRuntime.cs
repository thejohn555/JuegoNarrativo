using Blueprints_arquitectura_Dani_.Runtime;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.SetPlayerMovement
{
    public class SetPlayerMovementRuntime:RuntimeNode
    {
        public bool active;

        public SetPlayerMovementRuntime(bool active)
        {
            this.active = active;
        }
    }
}