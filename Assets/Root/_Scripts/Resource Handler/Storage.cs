using UnityEngine.Events;

namespace Root
{
    public interface IStorage
    {
        public Resource StorageResources { get; }

        public UnityEvent OnValueChanged { get; }

        public UnityEvent OnFull { get; }

        public int CurrentCount { get; }

        public int MaxCapacity { get; }

        public void Add(Resource resource);

    }

    public class Storage : IStorage
    {
        public Resource StorageResources { get; protected set; } = new Resource();

        public UnityEvent OnValueChanged { get; private set; } = new UnityEvent();
        public UnityEvent OnFull { get; private set; } = new UnityEvent();

        public int CurrentCount { get => StorageResources.Count(); }

        public int MaxCapacity { get; protected set; } = 0;

        public void Add(Resource resource)
        {
            StorageResources = Resource.Sum(StorageResources, resource);

            if (CurrentCount < MaxCapacity)
            {
                OnValueChanged.Invoke();
                return;
            }

            if (resource.MetalCount > 0)
            {
                StorageResources = Resource.Sum(StorageResources, Resource.Metal(MaxCapacity - CurrentCount));
            }
            if (resource.RubberCount > 0)
            {
                StorageResources = Resource.Sum(StorageResources, Resource.Rubber(MaxCapacity - CurrentCount));
            }
            if (resource.PlasticCount > 0)
            {
                StorageResources = Resource.Sum(StorageResources, Resource.Plastic(MaxCapacity - CurrentCount));
            }

            OnValueChanged.Invoke();
            OnFull.Invoke();
        }
    }
}