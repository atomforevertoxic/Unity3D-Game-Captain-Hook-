using UnityEngine.Events;

namespace Root
{
    public interface IRocketShip : IStorage
    {
        public UnityEvent OnHealthChanged { get; }
        public UnityEvent OnHealthZero { get; }
        public UnityEvent OnHealthFull { get; }

        public int Health { get; }
        public int MaxHealth { get; }

        public void TakeDamage(int damage);

        public void Heal(int extraHp);
    }

    public class RocketShip : Storage, IRocketShip
    {
        public UnityEvent OnHealthChanged { get; private set; } = new UnityEvent();
        public UnityEvent OnHealthZero { get; private set; } = new UnityEvent();
        public UnityEvent OnHealthFull { get; private set; } = new UnityEvent();

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        public RocketShip(int health, int maxHealth)
        {
            MaxCapacity = int.MaxValue;
            Health = health;
            MaxHealth = maxHealth;
        }

        public void Heal(int extraHp)
        {
            Health += extraHp;

            OnHealthChanged.Invoke();

            if (Health >= MaxHealth)
            {
                Health = MaxHealth;

                OnHealthFull.Invoke();
            }
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            OnHealthChanged.Invoke();

            if (Health <= 0)
            {
                Health = 0;

                OnHealthZero.Invoke();
            }
        }
    }
}