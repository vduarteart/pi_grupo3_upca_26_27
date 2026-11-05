using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;

public class StoreCameraLook: MonoBehaviour
{
    [Header("Look Limits")]
    [SerializeField] private float horizontalLimit = 5f;
    [SerializeField] private float verticalLimit = 2f;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 0.05f;
    [SerializeField] private float smoothTime = 0.12f;
    
    [Header("Cursor")]
    [SerializeField] private RectTransform cursorDot;
    
    [SerializeField] private CinemachineCamera storeCamera;
    
    [SerializeField] private CinemachineCamera tvCamera;

    private float currentYaw;
    private float currentPitch;
    
    private float targetYaw;
    private float targetPitch;

    private float yawVelocity;
    private float pitchVelocity;
    
    private RectTransform cursorCanvas;
    
    private Camera mainCamera;
    
    private MeshRenderer hoveredRenderer;
    
    private bool isTransitioning = false;
    
    public bool IsTransitioning => isTransitioning;
    
    private Quaternion transitionStartRotation;
    
    private Quaternion transitionTargetRotation;
    
    [SerializeField] private GameObject hoveredObject;
    
    private Material highlightMaterial;
    private float currentHighlightIntensity = 0f;
    private float targetHighlightIntensity = 0f;

    [SerializeField] private float highlightTransitionSpeed = 5f;
    
    [SerializeField] private Transform tvScreenTarget;

    private void Update()
    {
        Cursor.visible = false;
        
        UpdateCursorPosition();
        
        DetectObjectUnderCursor();
        
        HandleMouseClick();
        
        HandleMouseLook();
        
        UpdateHighlight();
        
    }

    private void Awake()
    {
        mainCamera = Camera.main;

        if (cursorDot != null)
            cursorCanvas = cursorDot.parent as RectTransform;
    }
    
    private void UpdateCursorPosition()
    {
        if (Mouse.current == null || cursorDot == null || cursorCanvas == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(cursorCanvas, mousePosition, null, out Vector2 localPosition))
        {
            cursorDot.anchoredPosition = localPosition;
        }
    }
    
    private void HandleMouseLook()
    {
        if (isTransitioning)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        targetYaw += mouseDelta.x * mouseSensitivity;
        
        targetPitch -= mouseDelta.y * mouseSensitivity;

        targetYaw = Mathf.Clamp(targetYaw, -horizontalLimit, horizontalLimit);

        targetPitch = Mathf.Clamp(targetPitch, -verticalLimit, verticalLimit);

        currentYaw = Mathf.SmoothDamp(currentYaw, targetYaw, ref yawVelocity, smoothTime);

        currentPitch = Mathf.SmoothDamp(currentPitch, targetPitch, ref pitchVelocity, smoothTime);

        transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
    
    private void DetectObjectUnderCursor()
    {
        if (mainCamera == null || Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        GameObject detectedObject = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            detectedObject = hit.collider.transform.parent.gameObject;
        }

        if (detectedObject == hoveredObject)
            return;

        if (hoveredObject != null)
        {
            SetHighlight(hoveredObject, false);
        }

        hoveredObject = detectedObject;

        if (hoveredObject != null)
        {
            SetHighlight(hoveredObject, true);
        }
    }
    
    private void SetHighlight(GameObject target, bool active)
    {
        if (target == null)
            return;

        MeshRenderer renderer = target.GetComponentInChildren<MeshRenderer>();

        if (renderer == null || renderer.materials.Length < 2)
            return;

        highlightMaterial = renderer.materials[1];
        targetHighlightIntensity = active? 0.5f : 0f;
    }
    
    private void UpdateHighlight()
    {
        if (highlightMaterial == null)
            return;

        currentHighlightIntensity = Mathf.Lerp(
            currentHighlightIntensity,
            targetHighlightIntensity,
            Time.deltaTime * highlightTransitionSpeed
        );

        highlightMaterial.SetFloat("_RimIntensity", currentHighlightIntensity);
    }
    
    private void HandleMouseClick()
    {
        if (Mouse.current == null || isTransitioning)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame && hoveredObject != null)
        {
            if (hoveredObject.name == "TV_01")
            {
                StartCoroutine(TransitionToTV());
            }
        }
    }
    
    private IEnumerator TransitionToTV()
    {
        isTransitioning = true;

        Transform cameraPoint = transform.parent;

        transitionStartRotation = cameraPoint.rotation;
        
        currentYaw = targetYaw;
        currentPitch = targetPitch;

        transitionTargetRotation =
            tvCamera.transform.rotation *
            Quaternion.Inverse(transform.localRotation);

        float duration = 2.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            cameraPoint.rotation = Quaternion.Slerp(
                transitionStartRotation,
                transitionTargetRotation,
                t
            );

            yield return null;
        }

        cameraPoint.rotation = transitionTargetRotation;

        storeCamera.Priority = 5;
        tvCamera.Priority = 10;

        yield return new WaitForSeconds(2.2f);

        isTransitioning = false;
    }
    
}
