using UnityEngine;

public class TVEntrance : MonoBehaviour
{
    public TVTransition transition;
    public CablePassage cablePassage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (cablePassage == null || !cablePassage.cableConnected)
            return;

        transition.MovePlayer(other.gameObject);
    }
}
