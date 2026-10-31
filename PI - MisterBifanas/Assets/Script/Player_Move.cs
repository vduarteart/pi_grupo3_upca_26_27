using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Player_Move : MonoBehaviour
{
    public float speed = 7f;
    public float jumpSpeed = 2f;
    public float gravity = -10f;

    public CharacterController characterController;
    private Vector3 Velocity;
    private bool Ground;

    public InputActionReference movement;
    public InputActionReference jump;

    private void OnEnable()
    {
        movement.action.Enable();
        jump.action.Enable();
    }

    private void OnDisable()
    {
        movement.action.Disable();
        jump.action.Disable();
    }

    void Update()
    {
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
            transform.forward = move;
        }

        if (Ground && jump.action.WasPressedThisFrame())
        {
            Velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }

        Velocity.y += gravity * Time.deltaTime;

        Vector3 CalculatedMove = move * speed + Vector3.up * Velocity.y;
        characterController.Move(CalculatedMove * Time.deltaTime);
    }
}