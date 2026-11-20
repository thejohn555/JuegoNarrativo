using Assets.Scripts.EventBuses;
using Assets.Scripts.EventBuses.Events;
using Audio.Enums;
using EventBuses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private EventBusSO eventBus;
    [SerializeField] private AudioDB audioDB;

    //source
    private AudioSource MusicSource;

    private AudioSource AmbienceSource;

    public override void Awake()
    {
        base.Awake();

        MusicSource = transform.Find("MusicSource").GetComponent<AudioSource>();
        AmbienceSource = transform.Find("AmbienceSource").GetComponent<AudioSource>();
    }

    private void Start()
    {
        eventBus.Subscribe<OnMusicPlay>(PlayMusic);
        eventBus.Subscribe<OnSFXPlay>(PlaySFX);
    }

    private void PlayMusic(OnMusicPlay args)
    {
        if (args.audioEvent == AudioEventsID.NULL)
        {
            //audioSource.clip = audioDB.MusicList[args.musicClip];
            //audioSource.Play();
        }
        else
        {
            //args.audioEvent.Play();
        }
    }

    private void PlaySFX(OnSFXPlay args)
    {
        if (args.audioEvent == AudioEventsID.NULL)
        {
            args.audioSource.PlayOneShot(audioDB.SfxList[args.sfxClip]);
        }
        else
        {
            audioDB.EventList[args.audioEvent].Play(args.audioSource, audioDB, StartCoroutine);
            
        }
    }

    public AudioDB GetDatabase()
    {
        return audioDB;
    }

    private IEnumerator iddd()
    {
        yield return null;
    }
}