using EventBuses;
using System;
using System.Collections.Generic;
using System.Text;


public struct OnAudioVarChange<T> where T :  Enum, IEvent
{
    public T value;

    public OnAudioVarChange(T value)
    {
        this.value = value;
    }
}

