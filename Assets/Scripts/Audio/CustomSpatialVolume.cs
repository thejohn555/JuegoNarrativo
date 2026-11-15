using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class CustomSpatialVolume : MonoBehaviour
{
     
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private float minVolume;
    [SerializeField] private float maxVolume;

    private AudioSource audioSource;

    private void Awake()
    {
       audioSource = GetComponent<AudioSource>(); 
    }


    private void Update()
    {
        if ((targetTransform.position - this.transform.position).magnitude < maxDistance)
        { 
            if ((targetTransform.position - this.transform.position).magnitude< minDistance)
            {
                audioSource.volume = maxVolume;
            }
            else
            {
                float percentage = (targetTransform.position - this.transform.position).magnitude / maxDistance;
                float value = minVolume + ((maxVolume - minVolume) * (1 - percentage));
                audioSource.volume = value;
            }
            
        }
        else
        {
            audioSource.volume = 0;
        }
    }

}
