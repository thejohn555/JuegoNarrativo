using System;
using System.Collections.Generic;

namespace BlackboardSystem
{
    [Serializable]
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>
    {
        private readonly string name;
        private readonly int hashedKey;

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
    public class BlackboardEntry<T>
    {
        public BlackboardKey Key { get; }
        public T Value { get; }
        public Type ValueType { get; }

        public BlackboardEntry(BlackboardKey key, T value)
        {
            Key = key;
            Value = value;
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

    public class Blackboard
    {
        private Dictionary<string, BlackboardKey> keyRegistry = new();
        private Dictionary<BlackboardKey, object> entries = new();

        public bool TryGetValue<T>(BlackboardKey key, out T value)
        {
            if (entries.TryGetValue(key, out object entry) && entry is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValue<T>(BlackboardKey key, T value)
        {
            entries[key] = new BlackboardEntry<T>(key, value);
        }

        public BlackboardKey GetOrRegisterKey(string keyName)
        {
            if (!keyRegistry.TryGetValue(keyName, out BlackboardKey key))
            {
                key = new BlackboardKey(keyName);
                keyRegistry[keyName] = key;
            }

            return key;
        }

        public bool ContainsKey(BlackboardKey key)
        {
            return entries.ContainsKey(key);
        }

        public void Remove(BlackboardKey key)
        {
            entries.Remove(key);
        }
    }
}