using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume",level);
    }
    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", level);

    }
    public void SetSoundVolume(float level)
    {
        audioMixer.SetFloat("SoundVolume", level);

    }
}
