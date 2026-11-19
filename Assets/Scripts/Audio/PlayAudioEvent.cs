using UnityEditor;
using UnityEngine;


public class PlayAudioEvent : MonoBehaviour
{
    [SerializeField] private AudioEvent audioEvent;

    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Play();
    }
    public void Play()
    {
        audioEvent.Play(audioSource, AudioManager.Instance.GetDatabase());
    }
}
