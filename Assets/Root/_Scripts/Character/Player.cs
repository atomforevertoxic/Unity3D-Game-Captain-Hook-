using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

namespace Root
{
    public interface IPlayer
    {
        public UnityEvent OnSpawn { get; }
        public UnityEvent OnOffSpawn { get; }

        public AudioSource LoopingSound_1 { get; }

        public AudioSource LoopingSound_2 { get; }

        public void UpgadeHook(int value);
    }

    public class Player : MonoBehaviour, IPlayer
    {
        [Header("Gameplay Settings")]
        [SerializeField] private float retractSpeed = 50f;
        [SerializeField] private float retractSpeedPlatformCancel = 80f;

        [Header("Sounds")]
        [Tooltip("Звук приземления на платформу")]
        [SerializeField] private AudioClip _onLandingSound;
        [Tooltip("Звук намотки крюка")]
        [SerializeField] private AudioClip _onRetractingSoundLoop;
        [Tooltip("Звук перезарядки")]
        [SerializeField] private AudioClip _onReadySound;
        [Tooltip("Звук попытки выстрелить при незаряженном крюке")]
        [SerializeField] private AudioClip _onInvalidShootSound;
        [Tooltip("Звук получения маленького мусора")]
        [SerializeField] private AudioClip _onSmallTrashReceived;
        [Tooltip("Звук получения среднего мусора")]
        [SerializeField] private AudioClip _onMediumTrashReceived;
        [Tooltip("Звук получения большого мусора")]
        [SerializeField] private AudioClip _onBigTrashReceived;
        [Tooltip("Звук получения астероида")]
        [SerializeField] private AudioClip _onAsteroidReceived;

        [Header("System Settings")]
        [SerializeField] private Hook hook;

        public UnityEvent OnSpawn { private set; get; } = new UnityEvent();
        public UnityEvent OnOffSpawn { private set; get; } = new UnityEvent();

        public AudioSource LoopingSound_1 { get => _hook.LoopingSound; }

        public AudioSource LoopingSound_2 { get => _isTransportingSound != null ? _isTransportingSound.GetComponent<AudioSource>() : null; }

        private IHook _hook;
        private IPlatform _platform;
        private IPlatform _spawnPlatform;

        private ILevelManager _levelManager;
        private ISoundFXManager _soundFXManager;

        private FirstPersonController _controller;

        private bool _isHookedToPlatform = false;
        public GameObject _isTransportingSound = null;

        private bool _isInCooldown = false;
        private float _cooldownTime = 0.1f;
        private float _timeSinceLastClick = 0f;

        private bool _isGame { get => _levelManager.IsGame; }
        private bool _isSpace { get => _levelManager.IsSpace; }
        private bool _isPaused { get => _levelManager.IsPaused; }

        private void Awake()
        {
            GetComponents();
        }

        private void Start()
        {
            GetElements();
            AddListeners();
        }

        private void Update()
        {
            UpdateTimer();
            AdjustPosition();
        }

        public void UpgadeHook(int value)
        {
            _hook.SetLength(_hook.Length + value);
        }

        private void GetComponents()
        {
            _controller = GetComponent<FirstPersonController>();
            _levelManager = Game.Instance.GetLevelManager();
            _levelManager.Constructor((IPlayer) this);
            _soundFXManager = Game.Instance.GetSoundFXManager();
        }

        private void GetElements()
        {
            _spawnPlatform = GameObject.FindWithTag("Respawn").GetComponent<IPlatform>();
            if (_spawnPlatform == null) Debug.LogError("SpawnPlatform not found");

            _platform = _spawnPlatform;
            if (_platform == null) Debug.LogError("Platform not found");

            CheckOnSpawn();

            _hook = hook.GetComponent<IHook>();
            if (_hook == null) Debug.LogError("Hook not found");

            _hook.IgnorePlatform = _platform;
        }

        private void AddListeners()
        {
            _controller.OnShoot.AddListener(HandleOnShoot);
            _controller.OnRetract.AddListener(HandleOnRetract);

            _hook.OnHooked.AddListener(HandleOnHooked);
            _hook.OnReady.AddListener(HandleOnReady);
        }

        private void HandleOnShoot()
        {
            if (!_isGame || _isPaused || _isInCooldown || !_isSpace) return;
            
            _isInCooldown = true;

            if (!IsHookMoving() && _hook.ItemHooked == null)
            {
                _soundFXManager.PlaySoundFXClip(_onInvalidShootSound, transform, 1f);
                _hook.Shoot();
            }
            else if (_isHookedToPlatform)
            {
                _platform = (IPlatform) _hook.ItemHooked;
                _hook.IgnorePlatform = _platform;
                _hook.ItemHooked = null;
                _isHookedToPlatform = false;

                StopSound();
                _isTransportingSound = _soundFXManager.InstantiateLoopingSoundFX(_onRetractingSoundLoop, transform, 1f);

                CheckOnSpawn();
            }
            else
            {
                _soundFXManager.PlaySoundFXClip(_onInvalidShootSound, transform, 1f);
            }
        }

        private bool IsHookMoving()
        {
            return _hook.IsMoving();
        }

        private void HandleOnRetract()
        {
            if (!_isGame || _isPaused || !_isSpace) return;

            if (_isHookedToPlatform && _isTransportingSound == null)
            {
                _hook.ItemHooked = null;
                _hook.Retract(retractSpeedPlatformCancel);
                _isHookedToPlatform = false;
            }
        }

        private void HandleOnHooked()
        {
            Debug.Log("Received event");

            if (_hook.ItemHooked is IPlatform)
            {
                _isHookedToPlatform = true;
                Debug.Log("Platform...");
            }
            else if (_hook.ItemHooked is ITrash)
            {
                _hook.Retract(retractSpeed);
                Debug.Log("Trash...");
            }
        }

        private void HandleOnReady()
        {
            if (_isTransportingSound != null)
            {
                Destroy(_isTransportingSound.gameObject);
                _isTransportingSound = null;
            }

            if (_hook.ItemHooked == null) return;

            if (_hook.ItemHooked is ITrash && ((ITrash)_hook.ItemHooked).trashType == Trash.TrashType.Asteroid)
            {
                _soundFXManager.PlaySoundFXClip(_onAsteroidReceived, transform, 1f);

                ((ITrash)_hook.ItemHooked).Destroy();

                _hook.ItemHooked = null;

                return;
            }

            if (_hook.ItemHooked is ITrash)
            {
                ITrash obj = (ITrash)_hook.ItemHooked;

                if (obj.Value == _levelManager.SmallAsteroidPrice)
                {
                    _soundFXManager.PlaySoundFXClip(_onSmallTrashReceived, transform, 1f);
                }
                else if (obj.Value == _levelManager.MediumAsteroidPrice)
                {
                    _soundFXManager.PlaySoundFXClip(_onMediumTrashReceived, transform, 1f);
                }
                else if (obj.Value == _levelManager.BigAsteroidPrice)
                {
                    _soundFXManager.PlaySoundFXClip(_onBigTrashReceived, transform, 1f);
                }

                _levelManager.AddToBackpack(obj.resource);

                obj.Destroy();

                _hook.ItemHooked = null;
            }
        }

        private void AdjustPosition()
        {
            if (_platform == null)
            {
                Debug.Log("Platform is null");
                return;
            }

            transform.position = Vector3.Lerp(transform.position, _platform.Position.position, Time.deltaTime * 3f);

            if (_isTransportingSound != null && Vector3.Distance(transform.position, _platform.Position.position) < 1f)
            {
                _hook.ItemHooked = null;
                _hook.Retract(retractSpeedPlatformCancel);
                _isHookedToPlatform = false;

                StopSound();

                _soundFXManager.PlaySoundFXClip(_onLandingSound, transform, 1f);
            }
        }

        private void CheckOnSpawn()
        {
            if (_platform == _spawnPlatform)
            {
                OnSpawn.Invoke();
            }
            else
            {
                OnOffSpawn.Invoke();
            }
        }

        private void UpdateTimer()
        {
            if (!_isInCooldown)
            {
                return;
            }

            _timeSinceLastClick += Time.deltaTime / Time.timeScale;

            if (_timeSinceLastClick >= _cooldownTime)
            {
                ResetCooldown();
            }
        }

        private void ResetCooldown()
        {
            _soundFXManager.PlaySoundFXClip(_onReadySound, transform, 1f);

            _isInCooldown = false;

            _timeSinceLastClick = 0f;
        }

        private void StopSound()
        {
            if (_isTransportingSound != null)
            {
                Destroy(_isTransportingSound.gameObject);
                _isTransportingSound = null;
            }
        }
    }
}
