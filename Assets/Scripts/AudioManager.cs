using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip keySound;
    [SerializeField] AudioClip doorOpenSound;
    [SerializeField] AudioClip doorLockedSound;
    [SerializeField] AudioClip levelCompletedSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayKeySound()
    {
        audioSource.PlayOneShot(keySound, 0.5f);
    }
    
    public void PlayDoorOpenSound()
    {
        audioSource.PlayOneShot(doorOpenSound, 1f);
    }

    public void PlayDoorLockedSound()
    {
        audioSource.PlayOneShot(doorLockedSound, 1f);
    }

    public void PlayLevelCompletedSound()
    {
        audioSource.PlayOneShot(levelCompletedSound, 0.33f);
    }
}
