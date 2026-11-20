using Assets.Scripts.EventBuses;
using EventBuses;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Audio.Enums;


[RequireComponent(typeof(AudioSource))]
public class PlayAudioEventOnAwake : MonoBehaviour
{
    [SerializeField] private EventBusSO eventBus;
    [SerializeField] private AudioEventsID id;

    private void Start()
    {
       eventBus.Publish<OnSFXPlay>(new OnSFXPlay(id, GetComponent<AudioSource>()));
    }
}

