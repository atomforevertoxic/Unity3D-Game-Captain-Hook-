using UnityEngine;

namespace Root
{
    public class SpaceObjectMovement : MonoBehaviour
    {
        //Вращение вокруг своей оси
        [SerializeField] protected Vector3 _axisRotation = new Vector3(0, 10, 0);

        [SerializeField] private float _pushForceMultiplier = 1;

        protected Vector3 _direction;

        private Rigidbody rb;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null) { Debug.Log("RB NULL!!!"); }
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            Debug.Log("BOUNCE!!!! " + gameObject.name + " " + collision.gameObject.name);
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();

            // Сюда перенести скрипт из RocketShipObject? Может заработать....

            _direction = (transform.position - collision.gameObject.transform.position);
            if (rb != null)
            {
                rb.AddForce(_direction * _pushForceMultiplier, ForceMode.Impulse);
            }
            
        }
    }
}
