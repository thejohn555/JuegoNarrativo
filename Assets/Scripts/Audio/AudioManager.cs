using Assets.Scripts.EventBuses;
using Assets.Scripts.EventBuses.Events;
using Audio.Enums;
using EventBuses;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private EventBusSO eventBus;
    [SerializeField] private AudioDB audioDB;
    [SerializeField] private AudioEventsDB audioEventsDB;

    //source
    private AudioSource audioSource;

    public override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        eventBus.Subscribe<OnMusicPlay>(PlayMusic);
        eventBus.Subscribe<OnSFXPlay>(PlaySFX);
    }


    public void PlayMusic(OnMusicPlay args)
    {
        if(args.audioEvent == AudioEventsID.NULL)
        {
            audioSource.clip = audioDB.MusicList[args.musicClip];
            audioSource.Play();
        }
        else
        {
            //args.audioEvent.Play();
        }
    }

    public void PlaySFX(OnSFXPlay args)
    {
        if (args.audioEvent == AudioEventsID.NULL)
        {
            audioSource.PlayOneShot(audioDB.SfxList[args.sfxClip]);
        }
        else
        {
            //args.audioEvent.Play();
        }
        
    }

}
