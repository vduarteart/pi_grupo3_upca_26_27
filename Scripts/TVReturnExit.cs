using UnityEngine;

public class TVReturnExit : MonoBehaviour
{
    public TVTransition transition;
    public TVPuzzleManager puzzleManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (puzzleManager == null || !puzzleManager.CableBConnected())
        {
            Debug.Log("Tens de ligar o Cabo B antes de regressar à TV1!");
            return;
        }

        if (transition != null)
        {
            transition.MovePlayer(other.gameObject);
        }
    }
}
