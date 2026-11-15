using UnityEngine;

public class CablePlugController: MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform cableStart;

    [Header("Cable Settings")]
    [SerializeField] private float maxCableLength = 3f;
    [SerializeField] private float returnSpeed = 5f;
    
    [SerializeField] private float magnetSpeed = 12f;

    private bool isDragging;
    private bool isReturning;
    private bool isConnected;

    private Plane dragPlane;
    private Vector3 dragOffset;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Transform nearbyPlugPosition;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
            TryStartDrag();

        if (isDragging)
        {
            DragPlug();

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Larguei a ficha. Tomada detetada: " + (nearbyPlugPosition != null));
                
                isDragging = false;

                if (nearbyPlugPosition != null)
                {
                    transform.SetPositionAndRotation(
                        nearbyPlugPosition.position,
                        nearbyPlugPosition.rotation *
                        Quaternion.Euler(0f, 180f, 0f)
                    );

                    isConnected = true;
                }
                else
                {
                    isReturning = true;
                }
            }
        }

        if (isReturning)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                initialPosition,
                returnSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                initialRotation,
                returnSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, initialPosition) < 0.01f)
            {
                transform.SetPositionAndRotation(
                    initialPosition,
                    initialRotation
                );

                isReturning = false;
            }
        }
    }

    private void TryStartDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.isTrigger)
                continue;

            if (hit.collider.transform.IsChildOf(transform) ||
                hit.collider.transform == transform)
            {
                isDragging = true;
                isConnected = false;

                dragPlane = new Plane(
                    -mainCamera.transform.forward,
                    transform.position
                );

                dragOffset = transform.position - hit.point;

                return;
            }
        }
    }

    private void DragPlug()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPosition = ray.GetPoint(distance) + dragOffset;

            // Atração magnética para a tomada
            if (nearbyPlugPosition != null)
            {
                targetPosition = Vector3.MoveTowards(
                    targetPosition,
                    nearbyPlugPosition.position,
                    magnetSpeed * Time.deltaTime
                );
            }

            // Limite do comprimento do cabo
            Vector3 direction = targetPosition - cableStart.position;

            if (direction.magnitude > maxCableLength)
            {
                targetPosition = cableStart.position +
                                 direction. normalized * maxCableLength;
            }

            transform.position = targetPosition;
        }
    }

    public void SetNearbySocket(Transform plugPosition)
    {
        nearbyPlugPosition = plugPosition;
    }

    public void ClearNearbySocket(Transform plugPosition)
    {
        if (nearbyPlugPosition == plugPosition)
            nearbyPlugPosition = null;
    }
}