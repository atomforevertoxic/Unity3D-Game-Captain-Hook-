using UnityEngine;

namespace Root
{
    public interface ISoundFXManager
    {
        void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume);
        public GameObject InstantiateMusic(AudioClip musicClip, Transform spawnTransform, float volume, bool play = true);
        GameObject InstantiateLoopingSoundFX(AudioClip audioClip, Transform spawnTransform, float volume);
    }

    public class SoundFXManager : IService, ISoundFXManager
    {
        public SoundFXManager()
        {
            GetSoundFXObject();
            GetMusicObject();
        }

        private AudioSource _soundFXObject;
        private AudioSource _musicObject;

        private static AudioSource _cachedSoundFXObject;
        private static AudioSource _cachedMusicObject;

        public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
        {
            if (_soundFXObject == null)
            {
                Debug.LogError("SoundFXObject is not initialized.");
                return;
            }

            AudioSource audioSource = MonoBehaviour.Instantiate(_soundFXObject, spawnTransform.position, Quaternion.identity);

            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();

            float clipLength = audioSource.clip.length;
            MonoBehaviour.Destroy(audioSource.gameObject, clipLength);
        }

        public GameObject InstantiateMusic(AudioClip musicClip, Transform spawnTransform, float volume, bool play = true)
        {
            if (_musicObject == null)
            {
                Debug.LogError("Cannot set music: MusicObject is not loaded.");
            }

            AudioSource audioSource = MonoBehaviour.Instantiate(_musicObject, spawnTransform.position, Quaternion.identity);

            audioSource.clip = musicClip;
            audioSource.volume = volume;
            audioSource.loop = true;

            if (play) audioSource.Play();

            return audioSource.gameObject;
        }

        public GameObject InstantiateLoopingSoundFX(AudioClip audioClip, Transform spawnTransform, float volume)
        {
            if (_soundFXObject == null)
            {
                Debug.LogError("SoundFXObject is not initialized.");
                return null;
            }

            AudioSource audioSource = MonoBehaviour.Instantiate(_soundFXObject, spawnTransform.position, Quaternion.identity);

            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.loop = true;
            audioSource.Play();

            return audioSource.gameObject;
        }

        private void GetSoundFXObject()
        {
            if (_cachedSoundFXObject == null)
            {
                _cachedSoundFXObject = Resources.Load<AudioSource>("SoundFXObject");
                if (_cachedSoundFXObject == null)
                {
                    Debug.LogError("SoundFXObject not found in Resources.");
                }
            }
            _soundFXObject = _cachedSoundFXObject;
        }

        private void GetMusicObject()
        {
            if (_cachedMusicObject == null)
            {
                _cachedMusicObject = Resources.Load<AudioSource>("MusicObject");
                if (_cachedMusicObject == null)
                {
                    Debug.LogError("MusicObject not found in Resources.");
                }
            }
            _musicObject = _cachedMusicObject;
        }
    }
}
