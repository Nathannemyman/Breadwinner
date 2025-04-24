using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer; //the audiomixer that controls the SFX & Music
    [Range(0f, 1f)] public float defaultSFXVolume = 1f;
    [Range(0f, 1f)] public float defaultMusicVolume = 1f;
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SetSFXVolume(defaultSFXVolume);
        SetMusicVolume(defaultMusicVolume);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    public float GetSFXVolume()
    {
        audioMixer.GetFloat("SFXVolume", out float volumeDb);
        return Mathf.Pow(10f, volumeDb / 20f);
    }

    public float GetMusicVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float volumeDb);
        return Mathf.Pow(10f, volumeDb / 20f);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
