using UnityEngine;
using UnityEngine.InputSystem;

public class StoreCameraController: MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3f;

    [Header("Movement Limits")]
    [SerializeField] private float horizontalLimit = 3f;
    [SerializeField] private float forwardLimit = 4f;
    [SerializeField] private float backwardLimit = 2f;

    [Header("Human Movement")]
    [SerializeField] private float bobFrequency = 1.5f;
    [SerializeField] private float verticalBobAmplitude = 0.025f;
    [SerializeField] private float lateralSwayAmplitude = 0.015f;
    [SerializeField] private float bobFadeSpeed = 3f;
    
    [SerializeField] private StoreCameraLook cameraLook;

    private Vector3 initialPosition;
    private Vector3 movementPosition;

    private float bobPhase;
    private float bobWeight;

    private void Start()
    {
        initialPosition = transform.position;
        movementPosition = initialPosition;
    }

    private void Update()
    {
        if (cameraLook != null && cameraLook.IsTransitioning)
            return;

        HandleMovement();
        HandleHumanMovement();
    }

    private void HandleMovement()
    {
        Vector2 movementInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                movementInput.x -= 1f;

            if (Keyboard.current.dKey.isPressed)
                movementInput.x += 1f;

            if (Keyboard.current.wKey.isPressed)
                movementInput.y += 1f;

            if (Keyboard.current.sKey.isPressed)
                movementInput.y -= 1f;
        }

        movementInput = Vector2.ClampMagnitude(movementInput, 1f);

        Vector3 movement = new Vector3(movementInput.x,0f,movementInput.y);

        movementPosition += movement * (movementSpeed * Time.deltaTime);

        movementPosition.x = Mathf.Clamp(movementPosition.x,initialPosition.x - horizontalLimit,initialPosition.x + horizontalLimit);

        movementPosition.z = Mathf.Clamp(movementPosition.z,initialPosition.z - backwardLimit,initialPosition.z + forwardLimit);

        movementPosition.y = initialPosition.y;

        transform.position = movementPosition;
    }

    private void HandleHumanMovement()
    {
        bool isMoving = false;

        if (Keyboard.current != null)
        {
            isMoving = Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed || Keyboard.current.sKey.isPressed || Keyboard.current.dKey.isPressed;
        }

        bobWeight = Mathf.MoveTowards(bobWeight,isMoving? 1f : 0f,Time.deltaTime * bobFadeSpeed);

        if (isMoving)
        {
            bobPhase += Time.deltaTime * bobFrequency * Mathf.PI * 2f;
        }

        float verticalBob = Mathf.Sin(bobPhase * 2f) * verticalBobAmplitude;

        float lateralSway = Mathf.Sin(bobPhase * 0.85f) * lateralSwayAmplitude;

        Vector3 bobOffset = Vector3.up * verticalBob + transform.right * lateralSway;

        transform.position = movementPosition + bobOffset * bobWeight;
    }
    
    public void SynchronizePosition()
    {
        movementPosition = transform.position;
        initialPosition = transform.position;
    }
}
