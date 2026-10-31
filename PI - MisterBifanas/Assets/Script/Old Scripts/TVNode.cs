using UnityEngine;

public class TVNode : MonoBehaviour
{
    public int tvNumber;

    private void OnMouseDown()
    {
        Debug.Log("Clique detetado na TV" + tvNumber);

        TVPuzzleManager manager =
            FindFirstObjectByType<TVPuzzleManager>();

        if (manager != null)
        {
            manager.SelectTV(this);
        }
        else
        {
            Debug.LogError("TVPuzzleManager não encontrado!");
        }
    }
}
