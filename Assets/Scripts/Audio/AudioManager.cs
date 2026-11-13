using System.Collections.Generic;
using UnityEngine;


#region enums 

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
    [SerializeField] private Dictionary<Music,  AudioClip> music = new();
    [SerializeField] private Dictionary<SFX, AudioClip> playerSfx = new();

    //sources
    private AudioSource playerSfxSource;
    private AudioSource musicSource;

    public override void Awake()
    {
        base.Awake();
        musicSource = transform.GetChild (0).GetComponent<AudioSource> ();
        playerSfxSource = transform.GetChild(1).GetComponent<AudioSource>();
    }

    void Start()
    {
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
        if (musicSource.clip != null)
        {
            playerSfxSource.PlayOneShot(playerSfx[name]);
        }
    }
    
}
