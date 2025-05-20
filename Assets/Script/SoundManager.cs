using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider MasterSlider, BgmSlider, SfxSlider;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void MasterAudioControl()
    {
        float volume = MasterSlider.value;

        if (volume <= -40f) audioMixer.SetFloat("Master", -80f);
        else audioMixer.SetFloat("Master", volume);
    }
    public void BgmAudioControl()
    {
        float volume = BgmSlider.value;

        if(volume <= -40f) audioMixer.SetFloat("Bgm", -80f);
        else audioMixer.SetFloat("Bgm", volume);
    }
    public void SfxAudioControl()
    {
        float volume = SfxSlider.value;

        if (volume <= -40f) audioMixer.SetFloat("Sfx", -80f);
        else audioMixer.SetFloat("Sfx", volume);
    }

    public void ToggleMuteVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
}
