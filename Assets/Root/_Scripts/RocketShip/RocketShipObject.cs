using UnityEngine;

namespace Root
{
    public class RocketShipObject : MonoBehaviour
    {

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Collides in ship");
            GameObject go = collision.gameObject;

            if (go.CompareTag("OrbitTrash"))
            {
                SpawnObjectManager.SetFalseObject(go);
            }
            else if (go.CompareTag("RandomTrash"))
            {
                Destroy(go);
            }
            //string goName = obj.name.Substring(0, obj.name.Length - 7);
            //if (goName.EndsWith("AsteroidTrash"))
            //{
            //    Debug.Log("Asteroid damages the spaceship");
            //    Trash trashScript = obj.GetComponent<Trash>();
            //    Game.Instance.GetLevelManager().RocketTakeDamage(trashScript.Value);
            //}
        }

    }
}
