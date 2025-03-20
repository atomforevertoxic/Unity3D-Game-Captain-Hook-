using UnityEngine;

namespace Root
{
    public interface ITrash : IHookable
    {
        Resource resource { get; }

        public int Value { get; }

        public Trash.TrashType trashType { get; }

        public void Destroy();
    }

    public class Trash : MonoBehaviour, ITrash
    {
        public enum TrashType
        {
            Plastic,
            Rubber,
            Metal,
            Asteroid
        }

        [SerializeField] private TrashType type;
        [SerializeField] private int _value = 1;

        public TrashType trashType { get => type; }

        public int Value { 
            get => _value; 
            set
            {
                _value = value>=0? value : value*(-1);
            }
        }

        public Resource resource { get => CreateResource(); }

        public Transform Position { get => transform; set => transform.position = value.position; }

        private void Awake()
        {
            CreateResource();
        }

        public void Destroy()
        {
            if (gameObject.CompareTag("OrbitTrash"))
            {
                SpawnObjectManager.SetFalseObject(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private Resource CreateResource()
        {
            Resource resource;

            if (type == TrashType.Plastic)
            {
                resource = Resource.Plastic(_value);
            }
            else if (type == TrashType.Rubber)
            {
                resource = Resource.Rubber(_value);
            }
            else if (type == TrashType.Metal)
            {
                resource = Resource.Metal(_value);
            }
            else
            {
                resource = new Resource();
            }

            return resource;
        }
    }
}