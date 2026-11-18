using Audio.Enums;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioEventsDB", menuName = "Scriptable Objects/Audio Scriptable Object/AudioEventsDB")]
public class AudioEventsDB : ScriptableObject
{
    [SerializeField] private Dictionary<AudioEventsID, AudioEvent> audioEventsList;

    public Dictionary<AudioEventsID, AudioEvent> AudioEventsList
    {
        get
        {
            return audioEventsList;
        }
    }
}