using UnityEngine;

namespace Root
{
    public class PlatformMovement : SpaceObjectMovement
    {
        [SerializeField] private float _rotateMultiplier = 1;
        protected virtual void Update()
        {
            AxisRotation();
        }

        protected virtual void AxisRotation()
        {
            transform.Rotate(_axisRotation * Time.deltaTime * _rotateMultiplier);
        }

        private new void OnCollisionEnter(Collision collision)
        {
            GameObject go = collision.gameObject;
            if (go.CompareTag("OrbitTrash"))
            {
                Debug.Log("Орбитальный удалился об платформу");
                SpawnObjectManager.SetFalseObject(go);
            }
            else if (go.CompareTag("RandomTrash"))
            {
                Debug.Log("Рандомный удалился об платформу" + gameObject.name + " " + go.name);
                Destroy(go);
            }
        }
    }
}
