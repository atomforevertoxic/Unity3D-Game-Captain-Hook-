using UnityEngine;

namespace Root
{
    public class RandomRectilinearMovement : PlatformMovement
    {
        [SerializeField] private float _maxThrust = 5;





        private void Start()
        {
            _direction = new Vector3(
                Random.Range(-_maxThrust, _maxThrust),
                Random.Range(-_maxThrust, _maxThrust),
                Random.Range(-_maxThrust, _maxThrust)
                );
        }

        protected override void Update() 
        { 
            base.Update();  
            transform.Translate(_direction * Time.deltaTime); 
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
        }

    }
}

