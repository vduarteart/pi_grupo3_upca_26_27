using UnityEngine;

public class TVTransition : MonoBehaviour
{
    public Transform destination;

    public void MovePlayer(GameObject player)
    {
        if (player == null || destination == null)
            return;

        player.transform.position = destination.position;
    }
}