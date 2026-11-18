using UnityEngine;

public class InputSocketController: MonoBehaviour
{
    [Header("Connection")]
    [SerializeField] private Transform plugPosition;

    private CablePlugController connectedPlug;

    public bool IsConnected => connectedPlug != null && connectedPlug.IsConnected;

    private void OnTriggerEnter(Collider other)
    {
        CablePlugController plug = other.GetComponent<CablePlugController>();

        if (plug == null)
            plug = other.GetComponentInParent<CablePlugController>();

        if (plug == null)
            return;

        plug.SetNearbySocket(plugPosition, this);

        Debug.Log("Ficha dentro da zona de encaixe!");
    }

    private void OnTriggerExit(Collider other)
    {
        CablePlugController plug = other.GetComponent<CablePlugController>();

        if (plug == null)
            plug = other.GetComponentInParent<CablePlugController>();

        if (plug == null)
            return;

        plug.ClearNearbySocket(plugPosition, this);

        Debug.Log("Ficha saiu da zona de encaixe!");
    }

    public void RegisterConnection(CablePlugController plug)
    {
        connectedPlug = plug;
    }

    public void RemoveConnection(CablePlugController plug)
    {
        if (connectedPlug == plug)
            connectedPlug = null;
    }
}