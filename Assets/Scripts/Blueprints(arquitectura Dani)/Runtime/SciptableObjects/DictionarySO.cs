using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DictionarySO", menuName = "Scriptable Objects/DictionarySO")]
public class DictionarySO : ScriptableObject
{
    [SerializeField] public List<GameObject> triggerObjects = new();      // persiste en la escena
    [SerializeField] public List<GameObject> interactObjects = new();
}

