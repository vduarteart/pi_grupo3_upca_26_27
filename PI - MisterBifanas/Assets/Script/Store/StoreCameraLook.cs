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

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera storeCamera;

    [Header("Presentation")]
    [SerializeField] private PresentationController presentationController;
    

    [Header("TV Entry")]
    [SerializeField] private float cameraBlendDuration = 2.2f;
    [SerializeField] private float tvEntryPushDistance = 0.25f;
    [SerializeField] private float tvEntryPushDuration = 1.2f;

    [Header("TV Static")]
    [SerializeField] private float staticTransitionSpeed = 2f;

    [Header("TV Image")]
    [SerializeField] private float imageVisibilityTransitionSpeed = 2f;

    [Header("Highlight")]
    [SerializeField] private float highlightTransitionSpeed = 5f;

    [Header("Interaction")]
    [SerializeField] private GameObject hoveredObject;

    private float currentYaw;
    private float currentPitch;

    private float targetYaw;
    private float targetPitch;

    private float yawVelocity;
    private float pitchVelocity;

    private RectTransform cursorCanvas;
    private Camera mainCamera;

    private bool isTransitioning = false;
    private bool isInsideTV = false;

    public bool IsTransitioning => isTransitioning;

    private Material highlightMaterial;

    private float currentHighlightIntensity = 0f;
    private float targetHighlightIntensity = 0f;

    private Material tvScreenMaterial;

    private float currentStaticStrength = 0.11f;
    private float targetStaticStrength = 0.11f;

    private float currentImageVisibility = 0f;
    private float targetImageVisibility = 0f;

    private Vector3 tvCameraTargetStartPosition;
    
    private TVController activeTV;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (cursorDot != null)
        {
            cursorCanvas = cursorDot.parent as RectTransform;
        }
        
    }

    private void Update()
    {
        if (isInsideTV &&
            !isTransitioning &&
            Keyboard.current != null &&
            Keyboard.current.tabKey.wasPressedThisFrame)
        {
            StartExitTV();
        }

        Cursor.visible = false;

        UpdateCursorPosition();
        DetectObjectUnderCursor();
        HandleMouseClick();
        HandleMouseLook();
        UpdateHighlight();
        UpdateStatic();
        UpdateImageVisibility();
    }

    private void UpdateCursorPosition()
    {
        if (Mouse.current == null ||
            cursorDot == null ||
            cursorCanvas == null)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cursorCanvas,
            mousePosition,
            null,
            out Vector2 localPosition))
        {
            cursorDot.anchoredPosition = localPosition;
        }
    }

    private void HandleMouseLook()
    {
        if (isTransitioning || isInsideTV)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        targetYaw += mouseDelta.x * mouseSensitivity;
        targetPitch -= mouseDelta.y * mouseSensitivity;

        targetYaw = Mathf.Clamp(
            targetYaw,
            -horizontalLimit,
            horizontalLimit
        );

        targetPitch = Mathf.Clamp(
            targetPitch,
            -verticalLimit,
            verticalLimit
        );

        currentYaw = Mathf.SmoothDamp(
            currentYaw,
            targetYaw,
            ref yawVelocity,
            smoothTime
        );

        currentPitch = Mathf.SmoothDamp(
            currentPitch,
            targetPitch,
            ref pitchVelocity,
            smoothTime
        );

        transform.localRotation = Quaternion.Euler(
            currentPitch,
            currentYaw,
            0f
        );
    }

    private void DetectObjectUnderCursor()
    {
        if (isInsideTV || isTransitioning) return;
        if (mainCamera == null || Mouse.current == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        GameObject detectedObject = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            TVController tvController =
                hit.collider.GetComponentInParent<TVController>();

            if (tvController != null)
                detectedObject = tvController.gameObject;
        }

        if (detectedObject == hoveredObject) return;

        if (hoveredObject != null)
            SetHighlight(hoveredObject, false);

        hoveredObject = detectedObject;

        if (hoveredObject != null)
            SetHighlight(hoveredObject, true);
    }

    private void SetHighlight(GameObject target, bool active)
    {
        if (target == null)
            return;

        MeshRenderer renderer = target.GetComponentInChildren<MeshRenderer>();

        if (renderer == null || renderer.materials.Length < 2)
            return;

        highlightMaterial = renderer.materials[1];

        targetHighlightIntensity = active ? 0.5f : 0f;
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

        highlightMaterial.SetFloat(
            "_RimIntensity",
            currentHighlightIntensity
        );
    }

    private void UpdateStatic()
    {
        if (tvScreenMaterial == null)
            return;

        currentStaticStrength = Mathf.Lerp(
            currentStaticStrength,
            targetStaticStrength,
            Time.deltaTime * staticTransitionSpeed
        );

        tvScreenMaterial.SetFloat(
            "_StaticStrength",
            currentStaticStrength
        );
    }

    public void SetStaticStrength(float strength)
    {
        targetStaticStrength = strength;
    }

    private void UpdateImageVisibility()
    {
        if (tvScreenMaterial == null)
            return;

        currentImageVisibility = Mathf.Lerp(
            currentImageVisibility,
            targetImageVisibility,
            Time.deltaTime * imageVisibilityTransitionSpeed
        );

        tvScreenMaterial.SetFloat(
            "_ImageVisibility",
            currentImageVisibility
        );
    }

    public void SetImageVisibility(float visibility)
    {
        if (tvScreenMaterial == null)
            return;

        currentImageVisibility = visibility;
        targetImageVisibility = visibility;

        tvScreenMaterial.SetFloat(
            "_ImageVisibility",
            visibility
        );
    }

    private void HandleMouseClick()
    {
        if (Mouse.current == null || isTransitioning || isInsideTV)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame && hoveredObject != null)
        {
            TVController tvController = hoveredObject.GetComponent<TVController>();

            if (tvController != null)
                StartCoroutine(TransitionToTV(tvController));
        }
    }

    private void StartExitTV()
    {
        if (isTransitioning || !isInsideTV)
            return;

        StartCoroutine(TransitionFromTV());
    }

    private IEnumerator TransitionToTV(TVController tvController)
    {
        activeTV = tvController;
        tvScreenMaterial = activeTV.ScreenRenderer.materials[0];

        tvScreenMaterial.SetFloat("_StaticStrength", currentStaticStrength);
        tvScreenMaterial.SetFloat("_ImageVisibility", currentImageVisibility);

        isTransitioning = true;
        targetStaticStrength = 0.3f;

        yield return StartCoroutine(presentationController.LoadTVScene(activeTV));
        
        tvScreenMaterial.SetTexture("_MainTex", activeTV.RenderTexture);

        SetCursorVisible(false);

        if (hoveredObject != null)
        {
            SetHighlight(hoveredObject, false);
            hoveredObject = null;
        }

        // Guardar a posição inicial da câmara selecionada.
        tvCameraTargetStartPosition = activeTV.CameraTarget.position;

        // Transição Cinemachine.
        storeCamera.Priority = 5;
        activeTV.TVCamera.Priority = 10;

        yield return new WaitForSeconds(cameraBlendDuration);

        presentationController.EnterTV();
        targetImageVisibility = 1f;

        Vector3 pushStartPosition = activeTV.CameraTarget.position;
        Vector3 pushTargetPosition = tvCameraTargetStartPosition +
                                     activeTV.TVCamera.transform.forward * tvEntryPushDistance;

        float elapsed = 0f;
        bool fullscreenStarted = false;

        while (elapsed < tvEntryPushDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / tvEntryPushDuration);

            activeTV.CameraTarget.position =
                Vector3.Lerp(pushStartPosition, pushTargetPosition, t);

            if (t >= 0.5f && !fullscreenStarted)
            {
                presentationController.ShowTVWorld(1.5f);
                fullscreenStarted = true;
            }

            yield return null;
        }

        activeTV.CameraTarget.position = pushTargetPosition;

        isInsideTV = true;
        isTransitioning = false;
    }

    private IEnumerator TransitionFromTV()
    {
        isTransitioning = true;

        targetStaticStrength = 0.11f;
        targetImageVisibility = 0f;

        // Fazer desaparecer primeiro a imagem fullscreen.
        presentationController.HideTVWorld(0.8f);

        yield return new WaitForSeconds(0.8f);

        // Regressar ao formato de apresentação da loja.
        presentationController.ExitTV();

        // Iniciar o recuo da câmara.
        storeCamera.Priority = 10;
        activeTV.TVCamera.Priority = 5;

        yield return new WaitForSeconds(cameraBlendDuration);

        activeTV.CameraTarget.position = tvCameraTargetStartPosition;

        isInsideTV = false;

        yield return StartCoroutine(presentationController.UnloadTVScene(activeTV));

        tvScreenMaterial = null;

        isTransitioning = false;

        SetCursorVisible(true);
    }

    private void SetCursorVisible(bool visible)
    {
        if (cursorDot != null)
        {
            cursorDot.gameObject.SetActive(visible);
        }
    }
}
