
using UnityEngine;

public class StoreFootsteps : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Footsteps")]
    [SerializeField] private float stepInterval = 0.45f;

    private float stepTimer;

    public void UpdateFootsteps(bool isMoving)
    {
        if (isMoving)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                audioSource.Play();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
            audioSource.Stop();
        }
    }
}
