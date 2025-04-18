using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        TryGetComponent(out audioSource);
    }

    public void PlayAudioClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
