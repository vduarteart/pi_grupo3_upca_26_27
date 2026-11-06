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
    
    [SerializeField] private PresentationController presentationController;

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
    
    private bool isInsideTV = false;
    
    public bool IsTransitioning => isTransitioning;
    
    private Quaternion transitionStartRotation;
    
    private Vector3 storePosition;
    
    private Quaternion transitionTargetRotation;
    
    [SerializeField] private GameObject hoveredObject;
    
    private Material highlightMaterial;
    private float currentHighlightIntensity = 0f;
    private float targetHighlightIntensity = 0f;

    [SerializeField] private float highlightTransitionSpeed = 5f;
    
    [SerializeField] private Transform tvScreenTarget;
    

    private void Update()
    {
        if (isInsideTV && !isTransitioning && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            StartExitTV();
        }
        
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
    
    private void StartExitTV()
    {
        if (isTransitioning || !isInsideTV)
            return;

        StartCoroutine(TransitionFromTV());
    }
    
    private IEnumerator TransitionToTV()
    {
        isTransitioning = true;

        Transform cameraPoint = transform.parent;
        
        storePosition = cameraPoint.position;

        transitionStartRotation = cameraPoint.rotation;
        
        Vector3 transitionStartPosition = cameraPoint.position;
        Vector3 transitionTargetPosition = tvCamera.transform.position;
        currentYaw = targetYaw;
        currentPitch = targetPitch;

        transitionTargetRotation = tvCamera.transform.rotation * Quaternion.Inverse(transform.localRotation);

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
            
            cameraPoint.position = Vector3.Lerp(
                transitionStartPosition,
                transitionTargetPosition,
                t
            );

            yield return null;
        }

        cameraPoint.rotation = transitionTargetRotation;
        
        cameraPoint.position = transitionTargetPosition;
        
        storeCamera.Priority = 5;
        tvCamera.Priority = 10;

        presentationController.EnterTV();

        yield return new WaitForSeconds(0.7f);

        presentationController.ShowTVWorld();

        yield return new WaitForSeconds(1.5f);
        
        isInsideTV = true;

        isTransitioning = false;
    }
    
    private IEnumerator TransitionFromTV()
    {
        isTransitioning = true;

        presentationController.HideTVWorld();

        yield return new WaitForSeconds(0.8f);
        
        presentationController.ExitTV();
        
        yield return new WaitForSeconds(0.1f);
        
        Transform cameraPoint = transform.parent;

        Vector3 transitionStartPosition = cameraPoint.position;
        Quaternion exitStartRotation = cameraPoint.rotation;

        float duration = 2.2f;
        float elapsed = 0f;
        
        storeCamera.Priority = 10;
        tvCamera.Priority = 5;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            cameraPoint.position = Vector3.Lerp(
                transitionStartPosition,
                storePosition,
                t
            );

            cameraPoint.rotation = Quaternion.Slerp(
                exitStartRotation,
                transitionStartRotation,
                t
            );

            yield return null;
        }
        
        cameraPoint.position = storePosition;
        cameraPoint.rotation = transitionStartRotation;

        isInsideTV = false;
        isTransitioning = false;
    }
    
}
