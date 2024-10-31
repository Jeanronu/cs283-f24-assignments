using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class Coin : MonoBehaviour
{
    public float disappearDuration = 0.5f; // Duration of the disappearing animation
    public float rotationSpeed = 500f;     // Speed of spinning animation during disappear
    public AudioClip collectSound;         // Sound effect to play on collection

    // Movement parameters for jumping
    public float minJumpHeight = 0.2f;     // Minimum jump height
    public float maxJumpHeight = 0.5f;     // Maximum jump height
    public float minJumpSpeed = 1f;         // Minimum speed of the jump
    public float maxJumpSpeed = 3f;         // Maximum speed of the jump
    private float jumpHeight;                // Jump height for this coin
    private float jumpSpeed;                 // Jump speed for this coin
    private float originalY;                 // Original Y position of the coin

    private AudioSource audioSource;
    private bool isCollected = false;
    private float jumpOffset;                // Offset for the jumping movement

    // Event triggered when the coin is collected
    public event Action<Coin> OnCollected;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; // Ensure it doesn't play automatically
        originalY = transform.position.y; // Store the original Y position

        // Randomize jump parameters
        jumpHeight = UnityEngine.Random.Range(minJumpHeight, maxJumpHeight);
        jumpSpeed = UnityEngine.Random.Range(minJumpSpeed, maxJumpSpeed);
    }

    void Update()
    {
        if (!isCollected)
        {
            // Calculate the jumping effect
            jumpOffset = Mathf.Sin(Time.time * jumpSpeed) * jumpHeight;
            transform.position = new Vector3(transform.position.x, originalY + jumpOffset, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Monedero monedero = other.GetComponent<Monedero>();

        if (monedero != null && !isCollected)
        {
            isCollected = true;               // Prevent multiple triggers
            monedero.CoinsCollected();        // Increase the coin count
            StartCoroutine(DisappearAndNotifySpawner()); // Start disappearing effect
        }
    }

    private IEnumerator DisappearAndNotifySpawner()
    {
        // Play the collection sound
        audioSource.clip = collectSound;
        audioSource.Play();

        // Start spinning and scaling down for the disappearing effect
        Vector3 originalScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < disappearDuration)
        {
            // Increase rotation for the spin effect after collection
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // Gradually scale down the coin
            float scale = Mathf.Lerp(1f, 0f, elapsedTime / disappearDuration);
            transform.localScale = originalScale * scale;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the coin is completely invisible
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);

        // Notify the spawner to respawn the coin
        OnCollected?.Invoke(this);
    }

    // Method to reset the coin for respawning
    public void ResetCoin()
    {
        isCollected = false;
        transform.localScale = Vector3.one; // Reset scale
        gameObject.SetActive(true);         // Reactivate the coin
    }
}
