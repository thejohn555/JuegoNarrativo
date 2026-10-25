using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackboardSystem
{
    [Serializable]
    public class BlackboardEntry<T>
    {
        public BlackboardKey Key { get; }
        public T Value { get; }
        public Type ValueType { get; }

        public BlackboardEntry(BlackboardKey key, T value)
        {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }

        public override bool Equals(object obj)
        {
            return obj is BlackboardEntry<T> other && other.Key == Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    [Serializable]
    public class BlackboardEntryData : ISerializationCallbackReceiver
    {
        public string keyName;
        public AnyValue.ValueType valueType;
        public AnyValue value;

        public void SetValueOnBlackboard(Blackboard blackboard)
        {
            BlackboardKey key = blackboard.GetOrRegisterKey(keyName);
            setValueDispatchTable[value.type](blackboard, key, value);
        }

        //Dispatch table
        private static Dictionary<AnyValue.ValueType, Action<Blackboard, BlackboardKey, AnyValue>> setValueDispatchTable = new()
            {
                { AnyValue.ValueType.Bool, (blackboard, key, anyValue) => blackboard.SetValue<bool>(key,anyValue) },
                { AnyValue.ValueType.Int, (blackboard, key, anyValue) => blackboard.SetValue<int>(key,anyValue) },
                { AnyValue.ValueType.Float, (blackboard, key, anyValue) => blackboard.SetValue<float>(key,anyValue) },
                { AnyValue.ValueType.String, (blackboard, key, anyValue) => blackboard.SetValue<string>(key,anyValue) },
                { AnyValue.ValueType.Vector3, (blackboard, key, anyValue) => blackboard.SetValue<Vector3>(key,anyValue) },
                { AnyValue.ValueType.GameObject, (blackboard,key,AnyValue)=> blackboard.SetValue<GameObject>(key,AnyValue) }

            };

        public void OnAfterDeserialize()
        {
            value.type = valueType;
        }

        public void OnBeforeSerialize()
        {
        }
    }
}