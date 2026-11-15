using UnityEngine;

public class InputSocketController: MonoBehaviour
{
    [Header("Connection")]
    [SerializeField] private Transform plugPosition;

    private void OnTriggerEnter(Collider other)
    {
        CablePlugController plug = other.GetComponent<CablePlugController>();

        if (plug == null)
            plug = other.GetComponentInParent<CablePlugController>();

        if (plug == null)
            return;

        plug.SetNearbySocket(plugPosition);

        Debug.Log("Ficha dentro da zona de encaixe!");
    }

    private void OnTriggerExit(Collider other)
    {
        CablePlugController plug = other.GetComponent<CablePlugController>();

        if (plug == null)
            plug = other.GetComponentInParent<CablePlugController>();

        if (plug == null)
            return;

        plug.ClearNearbySocket(plugPosition);

        Debug.Log("Ficha saiu da zona de encaixe!");
    }
}