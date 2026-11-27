using EventBuses;
using UnityEngine;

public struct OnBarkStateChangedEvent : IEvent
{
    public GameObject TargetNpc;
    public BarkStateSO NewBarkGraph;

    public OnBarkStateChangedEvent(GameObject TargetNpc, BarkStateSO BarkGraph)
    {
        this.TargetNpc = TargetNpc;
        this.NewBarkGraph = BarkGraph;
    }
}
