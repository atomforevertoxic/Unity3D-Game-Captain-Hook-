using UnityEngine;
using UnityEngine.Events;

namespace Root
{
    public class Grapple : MonoBehaviour
    {
        [Header("Sounds")]
        [Tooltip("Звук попадания в платформу")]
        [SerializeField] private AudioClip _onHookedPlatformSound;
        [Tooltip("Звук попадания в мусор / маленький астероид")]
        [SerializeField] private AudioClip _onHookedTrashSound;
        [Tooltip("Звук попадания в астероид-платформу")]
        [SerializeField] private AudioClip _onMissAsteroidSound;
        [Tooltip("Звук попадания в корабль")]
        [SerializeField] private AudioClip _onMissRocketShipSound;

        public UnityEvent OnHooked { get; private set; } = new UnityEvent();
        public UnityEvent OnMiss { get; private set; } = new UnityEvent();

        public IPlatform IgnorePlatform { get; set; } = null;
        public IHookable HookedItem { get; set; } = null;

        private ISoundFXManager _soundFXManager;

        private void Awake()
        {
            _soundFXManager = Game.Instance.GetSoundFXManager();
        }

        private void Update()
        {
            if (HookedItem is ITrash)
            {
                ((Trash)HookedItem).Position = transform;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (HookedItem != null) return;

            if (other.GetComponent<IPlatform>() != null && other.GetComponent<IPlatform>() == IgnorePlatform)
            {
                OnMiss.Invoke();
            }
            else if (other.GetComponent<IPlatform>() != null && other.GetComponent<IPlatform>() != IgnorePlatform)
            {
                HookedItem = other.GetComponent<IPlatform>();
                HandlePlatformCollision(other.GetComponent<IPlatform>());
            }
            else if (other.GetComponent<ITrash>() != null)
            {
                HookedItem = other.GetComponent<ITrash>();
                HandleTrashCollision(other.GetComponent<ITrash>());
            }
            else if (other.GetComponent<IPlatform>() != IgnorePlatform)
            {
                if (other.GetComponent<AsteroidObject>() != null)
                {
                    _soundFXManager.PlaySoundFXClip(_onMissAsteroidSound, transform, 1f);

                    Debug.Log("Hit asteroid");
                }
                else if (other.GetComponent<RocketShipObject>() != null)
                {
                    _soundFXManager.PlaySoundFXClip(_onMissRocketShipSound, transform, 1f);

                    Debug.Log("Hit rocketship");
                }

                Debug.Log("HitSomething");

                OnMiss.Invoke();
            }
        }

        private void HandlePlatformCollision(IPlatform platform)
        {
            _soundFXManager.PlaySoundFXClip(_onHookedPlatformSound, transform, 1f);

            OnHooked.Invoke();
        }

        private void HandleTrashCollision(ITrash trash)
        {
            _soundFXManager.PlaySoundFXClip(_onHookedTrashSound, transform, 1f);

            OnHooked.Invoke();
        }
    }
}
