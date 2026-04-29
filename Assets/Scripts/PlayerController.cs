using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;

    public float speed = 5.0f;

    private float powerupStrength = 15.0f;
    public bool hasPowerup;

    public GameObject powerupIndicator;

    // Jump
    public float jumpForce = 7.0f;
    private bool isOnGround = true;
    private bool justJumped = false;

    // Fast jump & fall
    public float gravityModifier = 2.5f;
    public float fallMultiplier = 3.5f;

    // Shockwave
    public float shockwaveForce = 15.0f;
    public float shockwaveRadius = 5.0f;

    // Landing particles
    public GameObject landingParticle;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        // Movement
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);

        // Jump (only if powerup)
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && hasPowerup)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            justJumped = true;
        }

        // Fast jump & fall
        if (playerRb.linearVelocity.y < 0)
        {
            // Falling faster
            playerRb.AddForce(Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime, ForceMode.Acceleration);
        }
        else if (playerRb.linearVelocity.y > 0)
        {
            // Shorter jump arc
            playerRb.AddForce(Vector3.up * Physics.gravity.y * (gravityModifier - 1) * Time.deltaTime, ForceMode.Acceleration);
        }

        // Keep powerup indicator on player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);
            StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        powerupIndicator.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Landing
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;

            // Only trigger shockwave if just jumped and have powerup
            if (hasPowerup && justJumped)
            {
                // Spawn landing particle effect
                if (landingParticle != null)
                {
                    Instantiate(landingParticle, transform.position + new Vector3(0, -0.5f, 0), Quaternion.identity);
                }

                DoShockwave();
                justJumped = false;
            }
        }

        // Powerup collision with enemy
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.transform.position - transform.position);
            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
        }
    }

    void DoShockwave()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, shockwaveRadius);

        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Enemy"))
            {
                Rigidbody enemyRb = nearbyObject.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    Vector3 awayFromPlayer = (nearbyObject.transform.position - transform.position);
                    enemyRb.AddForce(awayFromPlayer * shockwaveForce, ForceMode.Impulse);
                }
            }
        }
    }
}