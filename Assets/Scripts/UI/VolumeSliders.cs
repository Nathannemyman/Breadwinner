using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSliders : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [Space(15)]

    [Header("Volume Sliders")]
    [SerializeField] private Slider audioSlider_Master;
    [SerializeField] private Slider audioSlider_Music;
    [SerializeField] private Slider audioSlider_SFX;

    private const string MIXER_MASTER = "MasterVolume";
    private const string MIXER_MUSIC = "MusicVolume";
    private const string MIXER_SFX = "SFXVolume";
    private const float VOLUME_MAX = 20;
    private const float VOLUME_MIN = -80f;

    private void OnEnable()
    {
        audioSlider_Master.maxValue = audioSlider_Music.maxValue = audioSlider_SFX.maxValue = VOLUME_MAX;
        audioSlider_Master.minValue = audioSlider_Music.minValue = audioSlider_SFX.minValue = VOLUME_MIN;

        audioMixer.GetFloat(MIXER_MASTER, out float volume);
        audioSlider_Master.value = volume;

        audioMixer.GetFloat(MIXER_MUSIC, out volume);
        audioSlider_Music.value = volume;

        audioMixer.GetFloat(MIXER_SFX, out volume);
        audioSlider_SFX.value = volume;
    }

    public void SetMasterVolume(Slider slider)
    {
        audioMixer.SetFloat(MIXER_MASTER, slider.value);
    }

    public void SetMusicVolume(Slider slider)
    {
        audioMixer.SetFloat(MIXER_MUSIC, slider.value);
    }

    public void SetSFXVolume(Slider slider)
    {
        audioMixer.SetFloat(MIXER_SFX, slider.value);
    }
}
