using System;
using System.Collections.Generic;
using UnityEngine;

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
                ((Action<T>)d)(ev);
            }
        }
    }

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        Type t = typeof(T);
        if (!subs.TryGetValue(t, out List<Delegate> list))
        {
            list = new List<Delegate>();
            subs[t] = list;
        }
        list.Add(handler);
    }

    public void UnSubscribe<T>(Action<T> handler) where T : IEvent
    {
        Type t = typeof(T);
        if (subs.TryGetValue(t, out List<Delegate> list))
        {
            list.Remove(handler);
            if (list.Count == 0)
            {
                subs.Remove(t);
            }
        }
    }
}
