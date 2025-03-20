using UnityEngine;

public class BoundaryHandler : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Object collides the wall + " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("OrbitTrash"))
        {
            Debug.Log("Орбитальный и границы");
            SpawnObjectManager.SetFalseObject(collision.gameObject);
        }
        else
        { 
            Debug.Log("Рандомный и границы");
            Destroy(collision.gameObject);
        }
    }
}
