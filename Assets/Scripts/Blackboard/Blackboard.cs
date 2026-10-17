using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackboardSystem
{
    [Serializable]
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>
    {
        readonly string name;
        readonly int hashedKey;

        public BlackboardKey(string name)
        {
            this.name = name;
            hashedKey = name.ComputerFNV1aHash();
        }

        public bool Equals(BlackboardKey other)
        {
            return hashedKey == other.hashedKey;
        }

        public override bool Equals(object obj)
        {
            return obj is BlackboardKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return hashedKey;
        }

        public override string ToString()
        {
            return name;
        }

        public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs)
        {
            return lhs.hashedKey == rhs.hashedKey;
        }

        public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs)
        {
            return !(lhs == rhs);
        }
    }

    [Serializable]
    public class Blackboard
    {
        Dictionary<string, BlackboardKey> keyRegistry = new();
        Dictionary<BlackboardKey, object> entries = new();

        public BlackboardKey GetOrRegisterKey(string keyName)
        {
            if (!keyRegistry.TryGetValue(keyName, out BlackboardKey key))
            {
                key = new BlackboardKey(keyName);
                keyRegistry[keyName] = key;
            }

            return key;

        }
    }
}
