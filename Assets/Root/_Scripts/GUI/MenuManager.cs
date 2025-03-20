using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Root
{
    public class MenuManager : MonoBehaviour
    {
        public UnityEvent OnPause { get; private set; } = new UnityEvent();
        public UnityEvent OnResume { get; private set; } = new UnityEvent();

        public bool IsGame { get; private set; } = false;
        public bool IsSpace { get; private set; } = true;

        public bool _isOnSpawn = true;

        [Header("Sounds")]
        [SerializeField] private AudioClip _clickSound;
        [SerializeField] private AudioClip _upgradeSound;
        [SerializeField] private AudioClip _errorSound;
        [SerializeField] private AudioClip _fixSound;

        [Header("Music")]
        [SerializeField] private AudioClip _musicMenu;
        [SerializeField] private AudioClip _musicSpace;
        [SerializeField] private AudioClip _musicRocket;
        [SerializeField] private AudioClip _musicFinish;
        [SerializeField] private AudioClip _musicFail;

        [Header("System Settings")]
        [SerializeField] private GameObject _startPanel;
        [SerializeField] private GameObject _gamePanel;
        [SerializeField] private GameObject _gamePanelPause;
        [SerializeField] private GameObject _shipButtons;
        [SerializeField] private GameObject _shipPanel;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _optionsPanel;
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _lossPanel;

        [SerializeField] private GameObject _panelFullInventory;
        [SerializeField] private GameObject _panelExtremeHealth;
        [SerializeField] private GameObject _panelNotEnoughResources;

        [SerializeField] private PanelRepair _panelRepair;

        [SerializeField] public PanelResources _panelResources;
        [SerializeField] public PanelResources _panelShipResources;
        [SerializeField] private PanelResources _shipResources;
        [SerializeField] public ProgressSlider _sliderShip;

        private ILevelManager _levelManager;
        private ISoundFXManager _soundFXManager;
        private IAudioMixerManager _audioMixerManager;

        private GameObject _currentMainPanel;

        private bool _isPaused { get => _levelManager.IsPaused; }

        private Resource money { get => _levelManager.RocketValue; }

        private AudioSource musicMenu;
        private AudioSource musicSpace;
        private AudioSource musicRocket;
        private AudioSource musicFinish;
        private AudioSource musicFail;


        private void Awake()
        {
            GetComponents();
        }

        private void Start()
        {
            OnRocketShipHealthChanged(_levelManager._rocketShip.Health, _levelManager._rocketShip.MaxHealth);
            StartCoroutine(CoroutineTakeDamage());
        }

        private void Update()
        {
            _shipButtons.SetActive(_isOnSpawn);
            CheckInput();
        }

        private IEnumerator CoroutineTakeDamage()
        {
            while (true)
            {
                _levelManager.RocketTakeDamage(Random.Range(4, 8));

                float delay = Random.Range(50f, 70f);

                yield return new WaitForSeconds(delay);
            }
        }

        public void StartGame()
        {
            IsGame = true;

            musicMenu.Pause();

            musicRocket = _soundFXManager.InstantiateMusic(_musicRocket, transform, 1f, false).GetComponent<AudioSource>();

            musicSpace = _soundFXManager.InstantiateMusic(_musicSpace, transform, 1f).GetComponent<AudioSource>();

            HandleResume();

            _startPanel.SetActive(false);
            _gamePanel.SetActive(true);

            SetCursorState(true);
        }

        public void SetSoundFXVolume(float volume)
        {
            _audioMixerManager.SetSoundFXVolume(volume);
        }

        public void SetMasterVolume(float volume)
        {
            _audioMixerManager.SetMasterVolume(volume);
        }

        public void SetMusicVolume(float volume)
        {
            _audioMixerManager.SetMusicVolume(volume);
        }

        public void PlayClickSound()
        {
            _soundFXManager.PlaySoundFXClip(_clickSound, transform, 1f);
        }

        public void HandleOptionsBack()
        {
            if (!IsGame && _optionsPanel.activeSelf)
            {
                HandleOptionsBackToStart();
            }
            else if (IsGame && _optionsPanel.activeSelf)
            {
                HandleOptionsBackToPause();
            }
        }

        public void HandleOptionsBackToStart()
        {
            PlayClickSound();

            _optionsPanel.SetActive(false);
            _startPanel.SetActive(true);
        }

        public void HandleOptionsBackToPause()
        {
            PlayClickSound();

            _optionsPanel.SetActive(false);
            _pausePanel.SetActive(true);
        }

        public void HandlePause()
        {
            SetCursorState(false);
            PlayClickSound();

            _levelManager.PauseGameplay();

            _currentMainPanel.SetActive(false);
            _pausePanel.SetActive(true);
        }

        public void HandleResume()
        {
            SetCursorState(IsSpace);
            PlayClickSound();

            _levelManager.ResumeGameplay();

            _pausePanel.SetActive(false);
            _currentMainPanel.SetActive(true);
        }

        public void HandleTryAgain()
        {
            PlayClickSound();

            SceneLoader.LoadActiveScene();
        }

        public void HandleClickRepairRocket()
        {
            Debug.Log(money);
            Debug.Log(_panelRepair.GetRocketShipPrice());
            Debug.Log(money.Subtract(_panelRepair.GetRocketShipPrice()));

            if (money.Subtract(_panelRepair.GetRocketShipPrice()))
            {
                _soundFXManager.PlaySoundFXClip(_fixSound, transform, 1f);

                _levelManager.AddToRocket(_panelRepair.GetRocketShipPrice().Negative());
                _levelManager.RocketHeal(50);
            }
            else
            {
                StartCoroutine(OnNotEnoughResources());
            }
        }

        public void HandleClickUpgradeBackpack()
        {
            if (money.Subtract(_panelRepair.GetBackpackPrice()))
            {
                _soundFXManager.PlaySoundFXClip(_upgradeSound, transform, 1f);

                _levelManager.AddToRocket(_panelRepair.GetBackpackPrice().Negative());

                _panelRepair.ShowNextBackpackInfo();

                _levelManager.UpgradeBackpack();
            }
            else
            {
                StartCoroutine(OnNotEnoughResources());
            }
        }

        public void HandleClickUpgradeHook()
        {
            if (money.Subtract(_panelRepair.GetBackpackPrice()))
            {
                _soundFXManager.PlaySoundFXClip(_upgradeSound, transform, 1f);

                _levelManager.AddToRocket(_panelRepair.GetHookPrice().Negative());

                _panelRepair.ShowNextHookInfo();

                _levelManager.UpgradeHook();
            }
            else
            {
                StartCoroutine(OnNotEnoughResources());
            }
        }

        public void OnBackpackValueChanged(Resource resource, int maxCapacity)
        {
            _panelResources.UpdateData(resource, maxCapacity, true);
        }

        public IEnumerator OnNotEnoughResources()
        {
            Debug.Log("Not enough resources");

            _soundFXManager.PlaySoundFXClip(_errorSound, transform, 1f);

            _panelNotEnoughResources.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _panelNotEnoughResources.SetActive(false);
        }

        public IEnumerator OnBackpackFull()
        {
            _panelFullInventory.SetActive(true);
            yield return new WaitForSeconds(3f);
            _panelFullInventory.SetActive(false);
        }

        public void OnRocketShipValueChanged(Resource resource, int maxValue)
        {
            _panelRepair.UpdateBackpackData(resource);
            _panelRepair.UpdateHookData(resource);
            _panelRepair.UpdateRocketData(resource);

            _panelShipResources.UpdateData(resource, maxValue, false);
            _shipResources.UpdateData(resource, maxValue, false);
        }

        public void OnRocketShipHealthChanged(int value, int maxValue)
        {
            _panelRepair.UpdateRocketHealth(value, maxValue);
            _sliderShip.SetMaxValue(maxValue);
            _sliderShip.SetValue(value);
        }

        public IEnumerator OnRocketShipHealthExtreme()
        {
            _sliderShip.SetExtremeColor(true);
            _panelExtremeHealth.SetActive(true);
            yield return new WaitForSeconds(2f);
            _panelExtremeHealth.SetActive(false);
        }

        public void TriggerWinPanel()
        {
            musicSpace.Stop();
            musicRocket.Stop();

            musicFinish = _soundFXManager.InstantiateMusic(_musicFinish, transform, 1f).GetComponent<AudioSource>();

            SetCursorState(false);

            _gamePanel.SetActive(false);
            _shipPanel.SetActive(false);
            _winPanel.SetActive(true);
        }

        public void TriggerLossPanel()
        {
            musicSpace.Stop();
            musicRocket.Stop();

            musicFail = _soundFXManager.InstantiateMusic(_musicFail, transform, 1f).GetComponent<AudioSource>();

            SetCursorState(false);

            _panelExtremeHealth.SetActive(false);
            _gamePanel.SetActive(false);
            _shipPanel.SetActive(false);
            _lossPanel.SetActive(true);
        }

        private void GetComponents()
        {
            _panelRepair.SetUp();

            _levelManager = Game.Instance.GetLevelManager();
            _levelManager.Constructor(this);

            _audioMixerManager = Game.Instance.GetAudioMixerManager();

            _soundFXManager = Game.Instance.GetSoundFXManager();
            musicMenu = _soundFXManager.InstantiateMusic(_musicMenu, transform, 1f).GetComponent<AudioSource>();

            _currentMainPanel = _gamePanel;
        }

        private void CheckInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsGame && _optionsPanel.activeSelf)
                {
                    HandleOptionsBackToStart();
                }
                else if (IsGame && _optionsPanel.activeSelf)
                {
                    HandleOptionsBackToPause();
                }
                else if (IsGame && _pausePanel.activeSelf)
                {
                    HandleResume();
                }
                else if (IsGame && _currentMainPanel.activeSelf)
                {
                    HandlePause();
                }
            }
            else if (Input.GetKeyDown(KeyCode.E) && IsSpace && _shipButtons.activeSelf)
            {
                GoToShip();
            }
            else if (Input.GetKeyDown(KeyCode.E) && !IsSpace)
            {
                GoToSpace();
            }
            else if (Input.GetKeyDown(KeyCode.R) && IsSpace && _shipButtons.activeSelf)
            {
                TryToRepairShip();
            }

        }

        public void GoToShip()
        {
            musicSpace.Stop();
            musicRocket.Play();

            PlayClickSound();
            IsSpace = false;
            SetCursorState(false);
            _shipPanel.SetActive(true);
            _gamePanel.SetActive(false);
            _currentMainPanel = _shipPanel;
        }

        public void GoToSpace()
        {
            musicRocket.Stop();
            musicSpace.Play();

            PlayClickSound();
            IsSpace = true;
            SetCursorState(true);
            _gamePanel.SetActive(true);
            _shipPanel.SetActive(false);
            _currentMainPanel = _gamePanel;
        }

        public void TryToRepairShip()
        {
            HandleClickRepairRocket();
        }

        public static void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(IsGame && !_isPaused);
        }
    }
}