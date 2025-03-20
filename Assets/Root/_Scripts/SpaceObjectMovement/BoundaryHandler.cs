using UnityEngine;

public class BoundaryHandler : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Object collides the wall + " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("OrbitTrash"))
        {
            Debug.Log("����������� � �������");
            SpawnObjectManager.SetFalseObject(collision.gameObject);
        }
        else
        { 
            Debug.Log("��������� � �������");
            Destroy(collision.gameObject);
        }
    }
}
