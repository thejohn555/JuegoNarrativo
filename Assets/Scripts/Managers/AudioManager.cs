using System;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource MusicSource;
    [field:SerializeField] public AudioClip collectObjectSound {  get; private set; }
    [SerializeField] private AudioClip dialogueSound;

    public override void Awake()
    {
        base.Awake();
        MusicSource.loop = true;
        SFXSource.loop = false;
    }

    public void PlaySFX(AudioClip clip, float volume = 0.5f)
    {
        SFXSource.pitch = 1;
        SFXSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip, float volume = 0.5f)
    {
        MusicSource.clip = clip;
        MusicSource.volume = volume;
        MusicSource.Play();
    }

    public void PlayDialogueSound()
    {
        SFXSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        SFXSource.PlayOneShot(dialogueSound, 0.4f);
    }
}
