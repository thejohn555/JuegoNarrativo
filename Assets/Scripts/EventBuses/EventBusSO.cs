using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventBuses
{
    public interface IEvent
    { }

    [CreateAssetMenu(fileName = "EventBusSO", menuName = "Scriptable Objects/EventBusSO")]
    public class EventBusSO : ScriptableObject
    {
        private readonly Dictionary<Type, List<Delegate>> subs = new();

        public void Publish<T>(T ev) where T : IEvent
        {
            Type t = typeof(T);
            if (subs.TryGetValue(t, out List<Delegate> list))
            {
                Delegate[] copy = list.ToArray();
                foreach (Delegate d in copy)
                {
                    if (d is Action<T> handlerWithEvent)
                    {
                        handlerWithEvent(ev);
                    }
                    else if (d is Action handlerWithoutEvent)
                    {
                        handlerWithoutEvent();
                    }
                }
            }
        }

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            AddSubscription(typeof(T), handler);
        }

        public void Subscribe<T>(Action handler) where T : IEvent
        {
            AddSubscription(typeof(T), handler);
        }

        public void UnSubscribe<T>(Action<T> handler) where T : IEvent
        {
            RemoveSubscription(typeof(T), handler);
        }

        public void UnSubscribe<T>(Action handler) where T : IEvent
        {
            RemoveSubscription(typeof(T), handler);
        }

        private void AddSubscription(Type eventType, Delegate handler)
        {
            if (!subs.TryGetValue(eventType, out List<Delegate> list))
            {
                list = new List<Delegate>();
                subs[eventType] = list;
            }
            list.Add(handler);
        }

        private void RemoveSubscription(Type eventType, Delegate handler)
        {
            if (subs.TryGetValue(eventType, out List<Delegate> list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                {
                    subs.Remove(eventType);
                }
            }
        }
    }
}