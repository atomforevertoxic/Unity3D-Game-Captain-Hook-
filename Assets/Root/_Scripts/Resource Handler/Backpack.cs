namespace Root
{
    public interface IBackpack : IStorage
    {
        public Resource TakeAll();

        public void UpgradeMaxCapacity(int extraCapacity);
    }

    public class Backpack : Storage, IBackpack
    {
        public Resource TakeAll()
        {
            Resource value = StorageResources.GetResources();
            
            StorageResources = new Resource();

            OnValueChanged.Invoke();

            return value;
        }

        public void UpgradeMaxCapacity(int extraCapacity)
        {
            MaxCapacity += extraCapacity;
        }
    }
}