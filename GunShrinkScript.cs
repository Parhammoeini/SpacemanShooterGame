using UnityEngine;
using System.Collections;

public class GunShrinkScript : MonoBehaviour
{
    public float shrinkAmount = 0.1f;  // Amount to shrink each time space is pressed
    public float resetDelay = 1.0f;    // Time to wait before resetting the size
    public GameObject fireEffectPrefab; // The fire effect prefab
    public GameObject bulletPrefab;     // Your bullet prefab
    public float fireForce = 10.0f;
    public int bulletsPerFire = 5;
    public float fireRate = 0.1f;
    public float projectileLifetime = 1.0f; // Time before the projectile disappears
    public float moveSpeed = 5f; // Speed of movement
    public GameObject objectPrefab; // The object prefab to spawn
    public float spawnInterval = 2f; // Time between spawns
    public float objectMoveSpeed = 2f; // Speed of the objects

    private Vector3 originalScale;

    void Start()
    {
        // Store the original scale of the gun
        originalScale = transform.localScale;
        // Start the object spawning coroutine
        StartCoroutine(SpawnObjects());
    }

    void Update()
    {
        // Check if the space button is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Start the shrinking coroutine
            StartCoroutine(ShrinkAndFire());
        }

        // Handle movement
        HandleMovement();
    }

    void HandleMovement()
    {
        // Get input from arrow keys
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Calculate the new position
        Vector3 move = new Vector3(moveX, moveY, 0);

        // Update the position of the GameObject
        transform.position += move * moveSpeed * Time.deltaTime;
    }

    IEnumerator ShrinkAndFire()
    {
        // Reduce the x scale of the gun
        Vector3 scale = transform.localScale;
        scale.x -= shrinkAmount;

        // Make sure the scale doesn't go below zero
        if (scale.x < 0)
        {
            scale.x = 0;
        }

        // Apply the new scale
        transform.localScale = scale;

        // Instantiate the fire effect and move it
        FireEffect();

        // Fire the bullets in rapid succession
        StartCoroutine(FireBullets());

        // Wait for the reset delay
        yield return new WaitForSeconds(resetDelay);

        // Reset the scale to the original size
        transform.localScale = originalScale;
    }

    void FireEffect()
    {
        if (fireEffectPrefab != null)
        {
            GameObject fireEffect = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity);

            // Optionally adjust the position of the fire effect
            AdjustFirePosition(fireEffect);

            // Start a coroutine to destroy the fire effect after a set time
            StartCoroutine(DestroyProjectileAfterTime(fireEffect, projectileLifetime));
        }
    }

    IEnumerator FireBullets()
    {
        for (int i = 0; i < bulletsPerFire; i++)
        {
            // Instantiate the bullet prefab
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Ensure the bullet has a Rigidbody2D component
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Set the bullet's direction and speed
                Vector2 fireDirection = transform.right; // Fire from left to right
                rb.AddForce(fireDirection * fireForce, ForceMode2D.Impulse);

                // Optionally set the bullet’s gravity scale if needed
                rb.gravityScale = 0; // Set to a non-zero value if you want gravity to affect bullets
            }

            // Ensure the bullet has a Collider2D component
            Collider2D col = bullet.GetComponent<Collider2D>();
            if (col == null)
            {
                Debug.LogError("Bullet prefab is missing a Collider2D component!");
            }

            // Optionally adjust the position of the bullet
            AdjustBulletPosition(bullet);

            // Start a coroutine to destroy the bullet after a set time
            StartCoroutine(DestroyProjectileAfterTime(bullet, projectileLifetime));

            // Wait for the specified fire rate before firing the next bullet
            yield return new WaitForSeconds(fireRate);
        }
    }


    void AdjustFirePosition(GameObject fireEffect)
    {
        Vector3 offset = new Vector3(0.8f, 0.1f, 0); // Adjust as needed
        fireEffect.transform.position = transform.position + offset;
    }

    void AdjustBulletPosition(GameObject bullet)
    {
        Vector3 offset = new Vector3(0.8f, 0.1f, 0); // Adjust as needed
        bullet.transform.position = transform.position + offset;
    }

    IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        // Wait for the specified time
        yield return new WaitForSeconds(time);

        // Destroy the projectile
        if (projectile != null)
        {
            Destroy(projectile);
        }
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            // Instantiate the object at the right edge of the screen
            GameObject obj = Instantiate(objectPrefab, new Vector3(10, Random.Range(-4f, 4f), 0), Quaternion.identity);
            // Set the object's speed
            obj.GetComponent<ObjectMover>().moveSpeed = objectMoveSpeed;
            // Wait for the next spawn
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
