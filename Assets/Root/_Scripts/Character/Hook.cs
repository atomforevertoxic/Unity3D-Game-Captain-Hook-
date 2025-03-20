using UnityEngine;
using UnityEngine.Events;

namespace Root
{
    public interface IHookable
    {
        Transform Position { get; }
    }

    public interface IHook
    {
        UnityEvent OnShoot { get; }
        UnityEvent OnHooked { get; }
        UnityEvent OnMiss { get; }
        UnityEvent OnRetract { get; }
        UnityEvent OnReady { get; }

        IPlatform IgnorePlatform { get; set; }
        IHookable ItemHooked { get; set; }

        int Length { get; }

        public bool IsMoving();

        public void Shoot();

        public void Retract(float speed);

        public void SetLength(int newLength);

        public AudioSource LoopingSound {  get; }
    }

    public class Hook : MonoBehaviour, IHook
    {
        [Header("Gameplay Settings")]
        [SerializeField] private int _length = 20;
        [SerializeField] private float shootSpeed = 30f;
        [SerializeField] private float retractSpeed = 50f;

        [Header("Sounds")]
        [Tooltip("Выброс крюка")]
        [SerializeField] private AudioClip _onShootSound;
        [Tooltip("Звук размотки крюка")]
        [SerializeField] private AudioClip _onShootingSoundLoop;
        [Tooltip("Звук полной размотки крюка")]
        [SerializeField] private AudioClip _onMissFullLength;
        [Tooltip("Звук намотки крюка")]
        [SerializeField] private AudioClip _onRetractingSoundLoop;

        [Header("System Settings")]
        [SerializeField] private Player player;
        [SerializeField] private Transform startPoint;
        [SerializeField] private GameObject harpoonHead;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform lineRendererPoint;
        [SerializeField] private Transform rotationTarget;
        [SerializeField] private GameObject fakeHarpoonHead;
        [SerializeField] private GameObject realHarpoonHead;

        public UnityEvent OnShoot { private set; get; } = new UnityEvent();
        public UnityEvent OnHooked { private set; get; } = new UnityEvent();
        public UnityEvent OnMiss { private set; get; } = new UnityEvent();
        public UnityEvent OnRetract { private set; get; } = new UnityEvent();
        public UnityEvent OnReady { private set; get; } = new UnityEvent();

        public int Length { private set => _length = value; get => _length; }

        public IPlatform IgnorePlatform { get => _grapple.IgnorePlatform; set => _grapple.IgnorePlatform = value; }
        public IHookable ItemHooked { get => _grapple.HookedItem; set => _grapple.HookedItem = value; }

        public AudioSource LoopingSound { get => _sound != null ? _sound.GetComponent<AudioSource>() : null; }

        private Grapple _grapple;

        private Vector3 shootDirection;

        private bool isShooting = false;
        private bool isRetracting = true;
        private bool isTrashRetracting = false;
        private bool isReady = true;

        private float currentDistance = 0f;

        private GameObject _sound;

        private ISoundFXManager _soundFXManager;

        private void Awake()
        {
            GetComponents();
        }

        private void Update()
        {
            UpdateGrapple();
        }

        public bool IsMoving()
        {
            return isShooting || isRetracting || isTrashRetracting;
        }

        public void Shoot()
        {
            if (isShooting || isRetracting) return;

            isReady = false;

            _soundFXManager.PlaySoundFXClip(_onShootSound, transform, 1f);

            StopSound();
            _sound = _soundFXManager.InstantiateLoopingSoundFX(_onShootingSoundLoop, transform, 1f);

            ResetHarpoon();
            CalculateShootDirection();
            RotateTowardsTarget(360f);
            isShooting = true;
            OnShoot.Invoke();
        }

        public void Retract(float speed)
        {
            if (isRetracting) return;

            StopSound();
            _sound = _soundFXManager.InstantiateLoopingSoundFX(_onRetractingSoundLoop, transform, 1f);

            isShooting = false;
            isRetracting = true;
            retractSpeed = speed;
            OnRetract.Invoke();
        }

        public void SetLength(int newLength)
        {
            Length = newLength;
        }

        private void GetComponents()
        {
            _soundFXManager = Game.Instance.GetSoundFXManager();

            _grapple = harpoonHead.GetComponent<Grapple>();

            _grapple.OnHooked.AddListener(() => OnHooked.Invoke());
            _grapple.OnHooked.AddListener(StopSound);

            _grapple.OnMiss.AddListener(() => OnMiss.Invoke());
            _grapple.OnMiss.AddListener(StopSound);
            _grapple.OnMiss.AddListener(() => Retract(retractSpeed));

            OnReady.AddListener(() => EnableStaticHead(true));
        }

        private void UpdateGrapple()
        {
            EnableStaticHead(false);

            UpdateLineRenderer(lineRendererPoint.position, harpoonHead.transform.position);

            if (ItemHooked != null && !isTrashRetracting)
            {
                if (ItemHooked is IPlatform)
                {
                    return;
                }
                else
                {
                    Retract(retractSpeed);
                }
            }

            if (isShooting)
            {
                MoveHarpoonOutward();
            }
            else if (isRetracting || isTrashRetracting)
            {
                MoveHarpoonInward();
            }
            else
            {
                StopSound();
                CalculateShootDirection();
                if (player._isTransportingSound == null)
                {
                    AdjustPosition();
                    
                }
                EnableStaticHead(true);
                RotateTowardsTarget(360f);
                UpdateLineRenderer(lineRendererPoint.position, lineRendererPoint.position);
            }
        }

        private void ResetHarpoon()
        {
            harpoonHead.transform.position = startPoint.position;
            currentDistance = 0f;
        }

        private void CalculateShootDirection()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                shootDirection = (hitInfo.point - startPoint.position).normalized;
            }
            else
            {
                shootDirection = ray.direction.normalized;
            }
        }

        private void RotateTowardsTarget(float rotationSpeed)
        {
            if (rotationTarget == null) return;

            Quaternion targetRotation = rotationTarget.rotation;

            harpoonHead.transform.rotation = Quaternion.Slerp(
                harpoonHead.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }

        private void MoveHarpoonOutward()
        {
            float step = shootSpeed * Time.deltaTime;
            harpoonHead.transform.position += shootDirection * step;
            currentDistance += step;

            if (currentDistance >= Length)
            {
                isShooting = false;

                StopSound();

                _soundFXManager.PlaySoundFXClip(_onMissFullLength, transform, 1f);

                Retract(retractSpeed);

                OnMiss.Invoke();
            }
        }

        private void MoveHarpoonInward()
        {
            Vector3 directionToOrigin = (startPoint.position - harpoonHead.transform.position).normalized;
            float step = retractSpeed * Time.deltaTime;
            harpoonHead.transform.position += directionToOrigin * step;

            if (Vector3.Distance(harpoonHead.transform.position, startPoint.position) <= 2f)
            {
                isRetracting = false;
                isTrashRetracting = false;

                if (player._isTransportingSound == null)
                {
                    OnReady.Invoke();
                    isReady = true;
                }
            }
        }

        private void AdjustPosition()
        {
            harpoonHead.transform.position = Vector3.Lerp(harpoonHead.transform.position, startPoint.position, Time.deltaTime * 50f);
        }

        private void UpdateLineRenderer(Vector3 start, Vector3 end)
        {
            if (lineRenderer == null) return;

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        private void StopSound()
        {
            if (_sound != null)
            {
                Destroy(_sound.gameObject);
                _sound = null;
            }
        }

        private void EnableStaticHead(bool enable)
        {
            fakeHarpoonHead.SetActive(enable);
            realHarpoonHead.SetActive(!enable);
        }
    }
}

