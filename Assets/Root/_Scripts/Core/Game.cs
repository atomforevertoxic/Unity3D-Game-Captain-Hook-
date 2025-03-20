using Root;
using System;

namespace Root
{
    public class Game
    {
        public static Game Instance => _instance;

        private static Game _instance;

        private ServiceLocator _serviceLocator;

        private Game() { }

        public Game(ServiceLocator serviceLocator)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("Game instance already exists!");
            }

            _instance = this;
            _serviceLocator = serviceLocator;

            InitializeServices();
        }

        public ILevelManager GetLevelManager()
        {
            return _serviceLocator.GetService<LevelManager>();
        }

        public ISoundFXManager GetSoundFXManager()
        {
            return _serviceLocator.GetService<SoundFXManager>();
        }

        public IAudioMixerManager GetAudioMixerManager()
        {
            return _serviceLocator.GetService<AudioMixerManager>();
        }

        private void InitializeServices()
        {
            _serviceLocator.GetService<LevelManager>().Constructor();
        }
    }
}