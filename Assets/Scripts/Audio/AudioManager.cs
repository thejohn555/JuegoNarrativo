using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;


#region enums 
public enum Levels
{
    Tabern
}

public enum SFX
{
    Dialogue,
    Object
}

public enum Music
{

}
#endregion

public class AudioManager : Singleton<AudioManager>
{
    //SoundCompilations
    [Header("Sound Archive")]
    [SerializeField] private Dictionary<Levels, AudioClip> ambience = new();
    [SerializeField] private Dictionary<Music,  AudioClip> music = new();
    [SerializeField] private Dictionary<SFX, AudioClip> sfx = new();

    //sources
    private AudioSource sfxSource;
    private AudioSource ambienceSource;
    private AudioSource musicSource;

    public override void Awake()
    {
        base.Awake();
        musicSource = transform.GetChild (0).GetComponent<AudioSource> ();
        ambienceSource = transform.GetChild(1).GetComponent<AudioSource> ();
        sfxSource = transform.GetChild(2).GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayLevelAmbience(Levels.Tabern);
    }

    public void PlayLevelAmbience(Levels level)
    {
        ambienceSource.clip = ambience[level];
        ambienceSource.Play();
    }

    public void PlayMusic(Music name)
    {
        musicSource.clip = music[name];
        if (musicSource.clip != null)
        {
            musicSource.Play();
        }
    }

    public void PlaySFX(SFX name)
    {
        sfxSource.PlayOneShot(sfx[name]);
    }
    
}
