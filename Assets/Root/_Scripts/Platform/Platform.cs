using UnityEngine;

namespace Root
{
    public interface IPlatform : IHookable
    {

    }

    public class Platform : MonoBehaviour, IPlatform
    {
        public Transform Position { get => _playerPosition; private set => _playerPosition = value; }

        [SerializeField] private Transform _playerPosition;
    }
}
