using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        TVPuzzleManager manager =
            FindFirstObjectByType<TVPuzzleManager>();

        if (manager != null)
        {
            manager.CollectKey();
        }

        Destroy(gameObject);
    }
}
