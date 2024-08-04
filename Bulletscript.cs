using UnityEngine;

public class Bulletscript : MonoBehaviour
{
    void Start()
    {
        // Destroy the bullet after a certain time
        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle collision logic
        if (collision.CompareTag("Target"))
        {
            Destroy(collision.gameObject); // Destroy the target object
            Destroy(gameObject); // Destroy the bullet
        }
    }
}


