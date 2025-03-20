using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public interface ILevelManager
    {
        public void Constructor(IPlayer player);

        public void Constructor(MenuManager menuManager);

        public void Constructor(int shipHealth, int extremeHealth, int shipMaxHealth, int backpackMaxCapacity, int smallAsteroidPrice, int mediumAsteroidPrice, int bigAsteroidPrice);

        public void SetSounds(AudioClip onFullInventorySound, AudioClip onExtremeHealthSound);

        public void AddToBackpack(Resource resources);

        public void AddToRocket(Resource resources);

        public void RocketTakeDamage(int value);

        public void RocketHeal(int value);

        public Resource BackpackValue { get; }

        public Resource RocketValue { get; }

        public IRocketShip _rocketShip { get; }

        public void PauseGameplay();

        public void ResumeGameplay();

        public void UpgradeBackpack();
        public void UpgradeHook();

        public bool IsPaused { get; }
        public bool IsGame { get; }
        public bool IsSpace { get; }

        public int SmallAsteroidPrice { get; }
        public int MediumAsteroidPrice { get; }
        public int BigAsteroidPrice { get; }
    }

    public class LevelManager : IService, ILevelManager
    {
        public Resource BackpackValue { get => _backpack.StorageResources; }

        public Resource RocketValue { get => _rocketShip.StorageResources; }

        public bool IsPaused { get; private set; }
        public bool IsGame { get => _menuManager.IsGame; }
        public bool IsSpace { get => _menuManager.IsSpace; }

        public int SmallAsteroidPrice { get; private set; }
        public int MediumAsteroidPrice { get; private set; }
        public int BigAsteroidPrice { get; private set; }

        private IPlayer _player;
        private IBackpack _backpack;
        public IRocketShip _rocketShip { get; private set; }

        private ISoundFXManager _soundFXManager;
        private MenuManager _menuManager;

        private bool _isSpawn = true;

        private int _shipExtremeHealth;

        private AudioClip _onFullInventorySound;
        private AudioClip _onExtremeHealthSound;

        private GameObject _soundObject;

        private int _levelBackpack = 0;
        private List<int> _levelsBackpack = new List<int>
        {
            10, 20, 20, 20, 20
        };

        private int _levelHook = 0;

        private List<int> _levelsHook = new List<int>
        {
            20, 20, 20, 20, 20
        };

        public void Constructor()
        {
            _soundFXManager = Game.Instance.GetSoundFXManager();
        }

        public void Constructor(IPlayer player)
        {
            _player = player;

            _player.OnSpawn.AddListener(OnSpawn);
            _player.OnOffSpawn.AddListener(OnOffSpawn);
        }

        public void Constructor(MenuManager menuManager)
        {
            _menuManager = menuManager;

            _menuManager.OnPause.AddListener(PauseGameplay);
            _menuManager.OnResume.AddListener(ResumeGameplay);

            Debug.Log("LevelManager received MenuManager");
        }

        public void Constructor(int shipHealth, int extremeHealth, int shipMaxHealth, int backpackMaxCapacity, int smallAsteroidPrice, int mediumAsteroidPrice, int bigAsteroidPrice)
        {
            SmallAsteroidPrice = smallAsteroidPrice;
            MediumAsteroidPrice = mediumAsteroidPrice;
            BigAsteroidPrice = bigAsteroidPrice;

            _backpack = new Backpack();
            _backpack.UpgradeMaxCapacity(backpackMaxCapacity);

            _backpack.OnValueChanged.AddListener(OnBackpackValueChanged);
            _backpack.OnFull.AddListener(OnBackpackFull);

            _rocketShip = new RocketShip(shipHealth, shipMaxHealth);

            _shipExtremeHealth = extremeHealth;

            _rocketShip.OnValueChanged.AddListener(OnRocketShipValueChanged);
            _rocketShip.OnHealthChanged.AddListener(OnRocketShipHealthChanged);
            _rocketShip.OnHealthFull.AddListener(OnRocketShipHealthFull);
            _rocketShip.OnHealthZero.AddListener(OnRocketShipHealthZero);

            OnRocketShipHealthChanged();

            Debug.Log("LevelManager received LevelSettings");
        }

        public void SetSounds(AudioClip onFullInventorySound, AudioClip onExtremeHealthSound)
        {
            _onFullInventorySound = onFullInventorySound;
            _onExtremeHealthSound = onExtremeHealthSound;
        }

        public void AddToBackpack(Resource resources)
        {
            if (_isSpawn)
            {
                AddToRocket(resources);

                return;
            }

            if (_backpack.CurrentCount == _backpack.MaxCapacity)
            {
                Debug.Log("Backpack is full, resources discarded");

                return;
            }

            _backpack.Add(resources);
        }

        public void AddToRocket(Resource resources)
        {
            _rocketShip.Add(resources);
        }

        public void RocketHeal(int value)
        {
            _rocketShip.Heal(value);
        }

        public void RocketTakeDamage(int value)
        {
            _rocketShip.TakeDamage(value);
        }

        public void PauseGameplay()
        {
            PlayLoopingSounds(false);
            Time.timeScale = 0f;
            IsPaused = true;
        }

        public void ResumeGameplay()
        {
            PlayLoopingSounds(true);
            Time.timeScale = 1f;
            IsPaused = false;
        }

        public void UpgradeBackpack()
        {
            _backpack.UpgradeMaxCapacity(_levelsBackpack[_levelBackpack++]);
            OnBackpackValueChanged();
        }

        public void UpgradeHook()
        {
            _player.UpgadeHook(_levelsHook[_levelHook++]);
        }

        private void OnSpawn()
        {
            Debug.Log("Player is on spawn");

            _isSpawn = true;

            _rocketShip.Add(_backpack.TakeAll());

            _menuManager._isOnSpawn = true;
            _menuManager._panelResources.SetActivePanel(false);
            _menuManager._panelShipResources.SetActivePanel(true);
        }

        private void OnOffSpawn()
        {
            Debug.Log("Player is off spawn");

            _isSpawn = false;

            _menuManager._isOnSpawn = false;
            _menuManager._panelShipResources.SetActivePanel(false);
            _menuManager._panelResources.SetActivePanel(true);
        }

        private void OnBackpackValueChanged()
        {
            Debug.Log("OnBackpackValueChanged: " + _backpack.CurrentCount + ", resource: " + _backpack.StorageResources);

            _menuManager.OnBackpackValueChanged(_backpack.StorageResources, _backpack.MaxCapacity);
        }

        private void OnBackpackFull()
        {
            Debug.Log("OnBackpackFull: " + _backpack.CurrentCount + ", resource: " + _backpack.StorageResources);
            
            _soundFXManager.PlaySoundFXClip(_onFullInventorySound, _menuManager.transform, 1f);
            _menuManager.StartCoroutine(_menuManager.OnBackpackFull());
        }

        private void OnRocketShipValueChanged()
        {
            Debug.Log("OnRocketShipValueChanged: " + _rocketShip.CurrentCount + ", resource: " + _rocketShip.StorageResources);

            _menuManager.OnRocketShipValueChanged(_rocketShip.StorageResources, _rocketShip.MaxCapacity);
        }

        private void OnRocketShipHealthChanged()
        {
            Debug.Log("OnRocketShipHealthChanged: " + _rocketShip.Health);

            if (_menuManager != null) _menuManager.OnRocketShipHealthChanged(_rocketShip.Health, _rocketShip.MaxHealth);

            if (_rocketShip.Health <= _shipExtremeHealth)
            {
                _menuManager.StartCoroutine(_menuManager.OnRocketShipHealthExtreme());

                if (_soundObject == null)
                {
                    _soundObject = _soundFXManager.InstantiateLoopingSoundFX(_onExtremeHealthSound, _menuManager.transform, 1f);
                }
            }
            else
            {
                DestroySound();
            }
        }

        private void OnRocketShipHealthFull()
        {
            Debug.Log("OnRocketShipHealthFull: " + _rocketShip.Health);

            PauseGameplay();

            _menuManager.TriggerWinPanel();
        }

        private void OnRocketShipHealthZero()
        {
            Debug.Log("OnRocketShipHealthZero: " + _rocketShip.Health);

            DestroySound();

            PauseGameplay();

            _menuManager.TriggerLossPanel();
        }

        private void PlayLoopingSounds(bool enable)
        {
            if (enable)
            {
                if (_player.LoopingSound_1 != null) _player.LoopingSound_1.Play();
                if (_player.LoopingSound_2 != null) _player.LoopingSound_2.Play();
                if (_soundObject != null) _soundObject.GetComponent<AudioSource>().Play();
            }
            else
            {
                if (_player.LoopingSound_1 != null) _player.LoopingSound_1.Stop();
                if (_player.LoopingSound_2 != null) _player.LoopingSound_2.Stop();
                if (_soundObject != null) _soundObject.GetComponent<AudioSource>().Stop();
            }
        }

        private void DestroySound()
        {
            if (_soundObject != null)
            {
                _menuManager._sliderShip.SetExtremeColor(false);

                MonoBehaviour.Destroy(_soundObject);

                _soundObject = null;
            }
        }
    }
}