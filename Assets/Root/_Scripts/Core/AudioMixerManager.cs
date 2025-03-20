using UnityEngine;
using UnityEngine.Audio;

namespace Root
{
    public interface IAudioMixerManager
    {
        public void SetSoundFXVolume(float volume);
        public void SetMasterVolume(float volume);
        public void SetMusicVolume(float volume);

    }

    public class AudioMixerManager : IService, IAudioMixerManager
    {
        private AudioMixer _audioMixer;

        public AudioMixerManager()
        {
            _audioMixer = Resources.Load<AudioMixer>("MainMixer");

            if (_audioMixer == null)
            {
                Debug.LogError("MainMixer not found in Resources.");
            }
        }

        public void SetMasterVolume(float volume)
        {
            _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20f);
        }

        public void SetSoundFXVolume(float volume)
        {
            _audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(volume) * 20f);
        }

        public void SetMusicVolume(float volume)
        {
            _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
        }
    }
}