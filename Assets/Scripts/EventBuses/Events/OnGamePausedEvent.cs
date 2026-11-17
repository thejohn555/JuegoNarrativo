using EventBuses;
using UnityEngine;

public struct OnGamePausedEvent : IEvent
{
    public bool isPaused { get; private set; }

    public OnGamePausedEvent(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}
