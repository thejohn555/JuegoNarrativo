using EventBuses;
using UnityEngine;

public struct OnNPCStateChangedEvent : IEvent
{
    public string TargetNpcID;
    public string NewStateID;

    public OnNPCStateChangedEvent(string targetNpcID, string newStateID)
    {
        TargetNpcID = targetNpcID;
        NewStateID = newStateID;
    }
}
