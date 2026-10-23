using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackboardSystem
{
    //[CreateAssetMenu(fileName = "BlackboardData", menuName = "Scriptable Objects/BlackboardData")]
    public class BlackboardData : MonoBehaviour//ScriptableObject
    {
        public List<BlackboardEntryData> entries = new();

        public void SetValuesOnBlackboard(Blackboard blackboard)
        {
            foreach (BlackboardEntryData entry in entries)
            {
                entry.SetValueOnBlackboard(blackboard);
            }
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
                { AnyValue.ValueType.Vector3, (blackboard, key, anyValue) => blackboard.SetValue<Vector3>(key,anyValue) }
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
        { Int, Float, Bool, String, Vector3, GameObject }

        public ValueType type;

        //Storage
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public Vector3 vector3Value;
        public String stringValue;
        public GameObject gameObjectValue;

        //Operators overrides

        #region Implicit operators overload
        public static implicit operator bool(AnyValue value)
        {
            return value.ConvertValue<bool>();
        }

        public static implicit operator int(AnyValue value)
        {
            return value.ConvertValue<int>();
        }

        public static implicit operator float(AnyValue value)
        {
            return value.ConvertValue<float>();
        }

        public static implicit operator string(AnyValue value)
        {
            return value.ConvertValue<string>();
        }

        public static implicit operator Vector3(AnyValue value)
        {
            return value.ConvertValue<Vector3>();
        }

        public static implicit operator GameObject(AnyValue value)
        {
            return value.ConvertValue<GameObject>();
        }
        #endregion Implicit operators overload

        private T ConvertValue<T>()
        {
            switch (type)
            {
                case ValueType.Bool:
                    return AsBool<T>(boolValue);

                case ValueType.Int:
                    return AsInt<T>(intValue);
                case ValueType.Float:
                    return AsFloat<T>(floatValue);
                case ValueType.String:
                    return (T)(object)stringValue;
                case ValueType.Vector3:
                    return (T)(object)vector3Value;
                case ValueType.GameObject:
                    return (T)(object)gameObjectValue;

                default:
                    throw new NotSupportedException($"Not supported value type: {typeof(T)}");
            }
        }

        #region AsMethods
        private T AsBool<T>(bool value)
        {
            return typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
        }

        private T AsInt<T>(int value)
        {
            return typeof(T) == typeof(int) && value is T correctType ? correctType : default;
        }

        private T AsFloat<T>(float value)
        {
            return typeof(T) == typeof(float) && value is T correctType ? correctType : default;
        }
        #endregion
    }
}