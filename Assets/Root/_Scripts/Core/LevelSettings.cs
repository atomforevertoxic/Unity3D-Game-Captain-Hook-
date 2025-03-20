using UnityEngine;

namespace Root
{
    public class LevelSettings : MonoBehaviour
    {
        [Header("Gameplay")]
        [SerializeField] int shipHealth = 50;
        [SerializeField] int shipExtremeHealth = 25;
        [SerializeField] int shipMaxHealth = 200;
        [SerializeField] int backpackMaxCapacity = 25;

        [Header("TrashValue")]
        [SerializeField] int smallAsteroidPrice = 1;
        [SerializeField] int mediumAsteroidPrice = 2;
        [SerializeField] int bigAsteroidPrice = 3;

        [Header("Sounds")]
        [SerializeField] private AudioClip onFullInventorySound;
        [SerializeField] private AudioClip onExtremeHealthSound;

        private void Awake()
        {
            Game.Instance.GetLevelManager().Constructor(shipHealth, shipExtremeHealth, shipMaxHealth, backpackMaxCapacity, smallAsteroidPrice, mediumAsteroidPrice, bigAsteroidPrice);
            Game.Instance.GetLevelManager().SetSounds(onFullInventorySound, onExtremeHealthSound);
        }
    }
}
