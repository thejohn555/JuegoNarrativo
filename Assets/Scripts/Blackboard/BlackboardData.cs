using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackboardSystem
{
    [CreateAssetMenu(fileName = "BlackboardData", menuName = "Scriptable Objects/BlackboardData")]
    public class BlackboardData : ScriptableObject
    {
        public List<BlackboardEntryData> entries = new();

        public void SetValuesOnBlackboard(Blackboard blackboard)
        {
            foreach (BlackboardEntryData entry in entries)
            {

            }
        }
    }

    [Serializable]
    public class BlackboardEntryData : ISerializationCallbackReceiver
    {
        public string keyName;
        public AnyValue.ValueType valueType;
        public AnyValue value;

            //Dispatch table
            static Dictionary<AnyValue.ValueType, Action<Blackboard, BlackboardKey, AnyValue>> setValueDispatchTable = new()
            {
                { AnyValue.ValueType.Bool, (blackboard, key, anyValue) => blackboard.SetValue<bool>(key,anyValue) },


            };

        
        public void OnAfterDeserialize()
        {
            value.type = valueType;
        }

        public void OnBeforeSerialize()
        {
            
        }
    }

    [Serializable]
    public struct AnyValue
    {
        public enum ValueType
        { Int, Float, Bool, String, Vector3 }

        public ValueType type;

        //Storage
        public bool boolValue;

        public static implicit operator bool(AnyValue value)
        {
            return value.ConvertValue<bool>();
        }

        private T ConvertValue<T>()
        {
            switch (type)
            {
                case ValueType.Bool:
                    return AsBool<T>(boolValue);

                default:
                    throw new NotSupportedException($"Not supported value type: {typeof(T)}");
            }
        }

        private T AsBool<T>(bool value)
        {
            return typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
        }
    }
}