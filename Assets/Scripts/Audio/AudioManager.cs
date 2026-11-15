using Assets.Scripts.EventBuses;
using Assets.Scripts.EventBuses.Events;
using Audio.Enums;
using EventBuses;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private EventBusSO eventBus;
    //SoundCompilations
    [Header("Sound Archive")]
    [SerializeField] private Dictionary<Music,  AudioClip> music = new();
    [SerializeField] private Dictionary<SFX, AudioClip> sfx = new();

    //sources
    private AudioSource playerSfxSource;
    private AudioSource musicSource;



    public void PlayMusic(Music name)
    {
        musicSource.clip = music[name];
        if (musicSource.clip != null)
        {
            eventBus.Publish<OnMusicPlay>(new OnMusicPlay(music[name]));
        }
    }

    public void PlayMusic(AudioBank audioBank)
    {

    }

    public void PlaySFX(SFX name)
    {
        if (musicSource.clip != null)
        {
            eventBus.Publish<OnSFXPlay>(new OnSFXPlay(sfx[name]));
        }
    }
    
}
