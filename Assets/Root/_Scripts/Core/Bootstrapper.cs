using UnityEngine;

namespace Root
{
    public class Bootstrapper : MonoBehaviour
    {
        private static Bootstrapper _instance;
        private ServiceLocator _serviceLocator;
        private Game _game;

        private void Awake()
        {
            if (!NeedBootstrap()) return;

            InitSystemComponents();
            InitGame();
            LoadGameScene();
        }

        private bool NeedBootstrap() =>
            (_instance ??= this) == this;

        private void InitSystemComponents()
        {
            _serviceLocator = new ServiceLocator();

            _serviceLocator.RegisterService<AudioMixerManager>(new AudioMixerManager());
            _serviceLocator.RegisterService<SoundFXManager>(new SoundFXManager());
            _serviceLocator.RegisterService<LevelManager>(new LevelManager());
        }

        private void InitGame()
        {
            if (_game == null)
            {
                _game = new Game(_serviceLocator);
            }
        }

        private void LoadGameScene()
        {
            Time.timeScale = 0f;

            SceneLoader.Load(SceneLoader.SceneID.MAIN_MENU);
        }
    }
}


