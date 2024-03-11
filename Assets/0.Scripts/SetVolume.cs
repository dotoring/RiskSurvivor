using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public SoundMgr soundMgr;

    private void Start()
    {
        SetVolumeLevel(SoundMgr.soundLevel);
        GetComponent<Slider>().value = SoundMgr.soundLevel;
    }

    public void SetVolumeLevel(float sliderVal)
    {
        mixer.SetFloat("EffectSoundParam", Mathf.Log10(sliderVal) * 20);
        SoundMgr.soundLevel = sliderVal;
    }
}
