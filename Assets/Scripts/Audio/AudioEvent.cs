using Audio.Enums;
using EventBuses;
using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AudioEvent", menuName = "Scriptable Objects/Audio Scriptable Object/AudioEvent")]
public class AudioEvent : ScriptableObject
{
    [SerializeField] private EventBusSO eventBus;
    [SerializeField] private List<SFXID> sfx;
    [SerializeField] private SoundOptions soundOptions;
    [SerializeField]private VarOptions varOptions;
    [SerializeField] private bool loop;
    

    public void Play(AudioSource audioSource, AudioDB audioDB)
    {
        switch(soundOptions)
        {
            case SoundOptions.Concurrent:
            {
                    foreach(SFXID id in sfx)
                    audioSource.PlayOneShot(audioDB.SfxList[id]);

                    break;

            }


               
        }
    }
}
