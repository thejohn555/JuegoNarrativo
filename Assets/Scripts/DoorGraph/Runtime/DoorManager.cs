using System;
using System.Collections.Generic;
using DoorGraph.Editor;
using UnityEngine;

namespace DoorGraph
{
    public class DoorManager:MonoBehaviour
    {
        public static DoorManager Instance { get; private set; }

        [SerializeField] DoorGraphAsset asset;
        [SerializeField] List<GameObject> doorObjects = new();
        readonly Dictionary<string, GameObject> doorByName = new();

        public DoorRuntime Runtime { get; private set; }

#if UNITY_EDITOR
        public static event Action DoorsChanged;
#endif

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            

            if (asset != null)
                Runtime = new DoorRuntime(asset);
            RebuildLookup();
        }
        void RebuildLookup()
        {
            doorByName.Clear();
            foreach (var go in doorObjects)
                if (go != null) doorByName[go.name] = go;
        }

        //Añadir puertas para la herramienta de john
        public void AddDoor(GameObject go)
        {
            doorObjects.RemoveAll(g => g == null);

            if (!doorObjects.Contains(go))
            {
                doorObjects.Add(go);
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            DoorsChanged?.Invoke();
#endif
        }

        public IReadOnlyList<GameObject> GetDoors() => doorObjects;
        public int GetActiveOutput(GameObject door)
        {
            if (Runtime == null || door == null) return -1;
            return Runtime.TryGetActiveOutput(door.name, out int output) ? output : -1;
        }
        
        
       //Devuelve el GameObject al que apunta la puerta
        public GameObject GetTargetDoor(GameObject door)
        {
            if (Runtime == null || door == null) return null;

            int next = Runtime.Resolve(door.name);
            if (next < 0) return null;

            doorByName.TryGetValue(Runtime.IndexOf(next), out var target);
            return target;
        }
        
        //cambiar la salida activa true si se ha podido hacer false si no
        public bool SetActiveOutput(GameObject door, int output)
        {
            if (Runtime == null || door == null) return false;
            return Runtime.SetActiveOutput(door.name, output);
        }
    }
}