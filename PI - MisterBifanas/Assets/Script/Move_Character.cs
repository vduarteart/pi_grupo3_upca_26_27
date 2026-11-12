using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Move_Character : MonoBehaviour
{
    public float horizontal;
    public float speed = 0f;
    public float jumpingPower = 16f;
    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TrailRenderer trail;

    public InputActionReference movement;
    public InputActionReference jump;
    public InputActionReference dash;

    public bool canDash = true;
    public bool isDashing;
    public float dashingPower = 20f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 0.5f;

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

    // Update is called once per frame
    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }
}
