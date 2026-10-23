namespace Blueprints_arquitectura_Dani_.Runtime.TriggerNode
{
    public class TriggerEnterRuntimeNode:RuntimeNode
    {
        public string TargetVariable;

        public TriggerEnterRuntimeNode(string varName)
        {
            TargetVariable = varName;
        }
    }
}