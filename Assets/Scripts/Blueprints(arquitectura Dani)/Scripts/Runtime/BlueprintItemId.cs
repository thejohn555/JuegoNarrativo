using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public class BlueprintItemId : MonoBehaviour
    {
        [SerializeField] private string guid;
        public string Guid => guid;

#if UNITY_EDITOR
        public void SetGuid(string value) => guid = value;
#endif
    }
}