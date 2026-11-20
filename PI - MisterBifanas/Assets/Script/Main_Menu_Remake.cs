using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Main_Menu_Remake : MonoBehaviour
{
    public float MercyTimer = 1f;

    void Update()
    {
        if (MercyTimer > 0)
        {
            MercyTimer -= Time.deltaTime;
        }

        if (MercyTimer <= 0)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                SceneManager.LoadScene("Store");
            }

            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                Application.Quit();
            }
        }

    }
}