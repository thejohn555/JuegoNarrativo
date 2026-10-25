using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackboardSystem
{
    
    public class BlackboardData : MonoBehaviour
    {
        [SerializeField] private Blackboard blackboardRef;
        public List<BlackboardEntryData> entries = new();

        public void SetValuesOnBlackboard()
        {
            foreach (BlackboardEntryData entry in entries)
            {
                entry.SetValueOnBlackboard(blackboardRef);
            }
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

        #endregion AsMethods
    }
}