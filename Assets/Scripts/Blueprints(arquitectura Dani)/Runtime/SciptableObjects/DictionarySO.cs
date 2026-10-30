using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DictionarySO", menuName = "Scriptable Objects/DictionarySO")]
public class DictionarySO : ScriptableObject
{
    [field: SerializeField] public List<string> triggerNames { get; private set; } = new();
#if UNITY_EDITOR
    public void AddName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || triggerNames.Contains(name)) return;

        triggerNames.Add(name);
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif
}

