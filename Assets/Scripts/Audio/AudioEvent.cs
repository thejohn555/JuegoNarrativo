using Audio.Enums;
using EventBuses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioEvent", menuName = "Scriptable Objects/Audio Scriptable Object/AudioEvent")]
public class AudioEvent : ScriptableObject
{
    [SerializeField] private EventBusSO eventBus;
    [SerializeField] private List<SFXID> sfx;
    [SerializeField] private SoundOptions soundOptions;
    [SerializeField] private VarOptions varOptions;
    [SerializeField] private bool loop;

    [SerializeField] private bool usingVarAudio;
    [SerializeField] private AudioBankID bankType;

    private int varValue;
    private Dictionary<int, List<AudioClip>> bankValue = new Dictionary<int, List<AudioClip>>();

    public void Initialize()
    {
        
        //eventBus.Subscribe<OnAudioVarChange<AudioEnums>>();
    }

    public void Play(AudioSource audioSource, AudioDB audioDB)
    {
        bankValue = audioDB.AudioBanks[bankType];
        switch (usingVarAudio)
        {
            case true:
                switch (soundOptions)
                {
                    case SoundOptions.Concurrent:

                        foreach (AudioClip audioClip in bankValue[varValue])
                            audioSource.PlayOneShot(audioClip);
                        break;

                    case SoundOptions.Sequentially:

                        SequentialAudio(audioDB, audioSource);
                        break;

                    case SoundOptions.Random:

                        int index = UnityEngine.Random.Range(0, (bankValue[varValue].Count - 1));

                        audioSource.PlayOneShot(bankValue[varValue][index]);

                        break;

                    case SoundOptions.Single:
                        audioSource.PlayOneShot(bankValue[varValue][0]);
                        break;
                }

                break;

            case false :
                switch (soundOptions)
                {
                    case SoundOptions.Concurrent:

                        foreach (SFXID id in sfx)
                            audioSource.PlayOneShot(audioDB.SfxList[id]);
                        break;

                    case SoundOptions.Sequentially:

                        SequentialAudio(audioDB, audioSource);
                        break;

                    case SoundOptions.Random:

                        int index = UnityEngine.Random.Range(0, (sfx.Count - 1));

                        audioSource.PlayOneShot(audioDB.SfxList[sfx[index]]);

                        break;

                    case SoundOptions.Single:
                        audioSource.PlayOneShot(audioDB.SfxList[sfx[0]]);
                        break;
                }
                break;
        }
        
    }

    private IEnumerator SequentialAudio(AudioDB audioDB, AudioSource audioSource)
    {
        if(usingVarAudio == false)
        {
            for (int i = 0; i < sfx.Count; i++)
            {
                audioSource.PlayOneShot(audioDB.SfxList[sfx[i]]);
                yield return new WaitForSeconds(audioDB.SfxList[sfx[i]].length);
            }
        }
        else
        {
            for (int i = 0; i < bankValue[varValue].Count; i++)
            {
                audioSource.PlayOneShot(bankValue[varValue][i]);
                yield return new WaitForSeconds(bankValue[varValue][i].length);
            }
        }
        
    }
}