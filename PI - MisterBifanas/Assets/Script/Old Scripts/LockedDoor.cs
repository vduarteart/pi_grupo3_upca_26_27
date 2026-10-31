using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    private bool isOpen = false;

    public void TryOpen()
    {
        if (isOpen)
            return;

        TVPuzzleManager manager =
            FindFirstObjectByType<TVPuzzleManager>();

        if (manager == null)
            return;

        if (!manager.HasKey())
        {
            Debug.Log("Precisas da chave para abrir esta porta!");
            return;
        }

        isOpen = true;
        gameObject.SetActive(false);

        Debug.Log("Porta aberta! O Cabo C está acessível.");
    }
}
