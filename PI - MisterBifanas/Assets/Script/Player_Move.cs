using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Player_Move : MonoBehaviour
{
    public float speed = 7f;
    public float jumpSpeed = 2f;
    public float gravity = -10f;

    public float dashRecharge = 1f;
    public float dashTime = 5f;
    public float dashSpeed = 4f;
    public bool dashAvailable;
    public bool dashing = false;

    public CharacterController characterController;
    [SerializeField] private Rigidbody rigidbody;

    public bool Ground;

    public InputActionReference movement;
    public InputActionReference jump;
    public InputActionReference dash;

    private void OnEnable()
    {
        movement.action.Enable();
        jump.action.Enable();
        dash.action.Enable();
    }

    private void OnDisable()
    {
        movement.action.Disable();
        jump.action.Disable();
        dash.action.Disable();
    }

    void Start()
    {
        dashAvailable = true;

        rigidbody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (dashing)
        {
            return;
        }

        Ground = characterController.isGrounded;
        //&& rigidbody.linearVelocity.y < -2f
        if (Ground)
        {
            rigidbody.linearVelocity = new Vector3(0f, -2f, 0f);
        }

        //Move Code

        Vector2 input = movement.action.ReadValue<Vector2>();

        if (input.x > 0)
        {
            rigidbody.linearVelocity = new Vector3(1 * speed, 0f, 0f);
        }

        if (input.x == 0)
        {
            rigidbody.linearVelocity = new Vector3(0f, 0f, 0f);
        }

        if (input.x < 0)
        {
            rigidbody.linearVelocity = new Vector3(-1 * speed, 0f, 0f);
        }

        //Jump Code

        if (Ground && jump.action.WasPressedThisFrame())
        {
            rigidbody.linearVelocity = new Vector3(0f, Mathf.Sqrt(jumpSpeed * -2f * gravity), 0f);
        }

        //Dash Code

        Vector3 move = new Vector3(input.x, 0, 0);
        move = Vector3.ClampMagnitude(move, 1f);

        Vector3 CalculatedMove = move * speed;

        if (dash.action.WasPressedThisFrame() && CalculatedMove.x > 0 && dashAvailable == true)
        {
            StartCoroutine(DashCoroutineRight());
        }

        if (dash.action.WasPressedThisFrame() && CalculatedMove.x < 0 && dashAvailable == true)
        {
            StartCoroutine(DashCoroutineLeft());
        }
    }

    public IEnumerator DashCoroutineRight()
    {
        dashAvailable = false;
        dashing = true;
        float normalGravity = gravity;
        gravity = 0f;
        rigidbody.linearVelocity = new Vector3(1 * dashSpeed, 0f, 0f);
        Debug.Log(rigidbody.linearVelocity);
        yield return new WaitForSeconds(dashTime);
        gravity = normalGravity;
        dashing = false;
        yield return new WaitForSeconds(dashRecharge);
        dashAvailable = true;
    }

    public IEnumerator DashCoroutineLeft()
    {
        dashAvailable = false;
        dashing = true;
        float normalGravity = gravity;
        gravity = 0f;
        rigidbody.linearVelocity = new Vector3(-1 * dashSpeed, 0f, 0f);
        yield return new WaitForSeconds(dashTime);
        gravity = normalGravity;
        dashing = false;
        yield return new WaitForSeconds(dashRecharge);
        dashAvailable = true;
    }
}

/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Player_Move : MonoBehaviour
{
    public float speed = 7f;
    public float jumpSpeed = 2f;
    public float gravity = -10f;

    public bool dashAvailable;
    public bool dashing = false;
    public float dashRecharge = 1f;
    public float dashTime = 5f;
    public float dashSpeed = 4f;

    public CharacterController characterController;
    private Vector3 Velocity;
    private bool Ground;

    public InputActionReference movement;
    public InputActionReference jump;
    public InputActionReference dash;

    private void OnEnable()
    {
        movement.action.Enable();
        jump.action.Enable();
        dash.action.Enable();
    }

    private void OnDisable()
    {
        movement.action.Disable();
        jump.action.Disable();
        dash.action.Disable();
    }

    void Start()
    {
        dashAvailable = true;
    }

    public void Update()
    {
        if (dashing)
        {
            return;
        }

        Ground = characterController.isGrounded;
        if (Ground && Velocity.y < -2f)
        {
            Velocity.y = -2f;
        }

        Vector2 input = movement.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, 0);
        move = Vector3.ClampMagnitude(move, 1f);
        
        if (move != Vector3.zero)
        {
            transform.right = move;
        }
        
        if (Ground && jump.action.WasPressedThisFrame())
        {
            Velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }

        Velocity.y += gravity * Time.deltaTime;

        Vector3 CalculatedMove = move * speed + Vector3.up * Velocity.y;
        characterController.Move(CalculatedMove * Time.deltaTime);
        
        Debug.Log(CalculatedMove);

        if (dash.action.WasPressedThisFrame() && CalculatedMove.x > 0 && dashAvailable == true)
        {
            StartCoroutine(DashCoroutineRight());
        }

        if (dash.action.WasPressedThisFrame() && CalculatedMove.x < 0 && dashAvailable == true)
        {
            StartCoroutine(DashCoroutineLeft());
        }
    }

    public IEnumerator DashCoroutineRight()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            dashAvailable = false;
            dashing = true;
            float normalGravity = gravity;
            gravity = 0f;
            transform.Translate(transform.right * dashSpeed * Time.deltaTime);
            gravity = normalGravity;
            dashing = false;
            yield return new WaitForSeconds(dashRecharge);
            dashAvailable = true;
        }
    }

    public IEnumerator DashCoroutineLeft()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            dashAvailable = false;
            dashing = true;
            float normalGravity = gravity;
            gravity = 0f;
            transform.Translate(-transform.right * dashSpeed * Time.deltaTime);
            gravity = normalGravity;
            dashing = false;
            yield return new WaitForSeconds(dashRecharge);
            dashAvailable = true;
        }
    }
}

/*

        if (CalculatedMove.x > 0 && movement.action.WasPressedThisFrame())
        {
            rigidbody.linearVelocity = new Vector3(1 * speed, 0f, 0f);
        }

        if (CalculatedMove.x < 0 && movement.action.WasPressedThisFrame())
        {
            rigidbody.linearVelocity = new Vector3(-1 * speed, 0f, 0f);
        }

        if (movement.action.WasReleasedThisFrame())
        {
            rigidbody.linearVelocity = new Vector3(0f, 0f, 0f);
        }


        input = Vector3.ClampMagnitude(input, 1f);
        Debug.Log(input.magnitude);
        rigidbody.linearVelocity = new Vector3(input.magnitude * speed, 0f, 0f);

      
 */