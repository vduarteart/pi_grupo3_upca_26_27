using UnityEngine;

public class CablePassage : MonoBehaviour
{
    public bool cableConnected = false;

    [Header("Bloqueadores das duas televisões")]
    [SerializeField] private GameObject blocker;
    [SerializeField] private GameObject secondBlocker;

    void Start()
    {
        UpdatePassage();
    }

    public void UpdatePassage()
    {
        if (blocker != null)
        {
            blocker.SetActive(!cableConnected);
        }

        if (secondBlocker != null)
        {
            secondBlocker.SetActive(!cableConnected);
        }
    }

    public void ConnectCable()
    {
        cableConnected = true;
        UpdatePassage();
    }

    public void DisconnectCable()
    {
        cableConnected = false;
        UpdatePassage();
    }
}