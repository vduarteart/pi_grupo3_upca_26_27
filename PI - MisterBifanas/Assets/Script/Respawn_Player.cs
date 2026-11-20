using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Respawn_Player : MonoBehaviour
{
    public GameObject spawn;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Kill"))
        {
            transform.position = spawn.transform.position;
        }
    }
}