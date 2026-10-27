using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource MusicSource;

    public override void Awake()
    {
        base.Awake();
        MusicSource.loop = true;
        SFXSource.loop = false;
    }

    public void PlaySFX(AudioClip clip, float volume = 0.5f)
    {
        SFXSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip, float volume = 0.5f)
    {
        MusicSource.clip = clip;
        MusicSource.volume = volume;
        MusicSource.Play();
    }
}
