using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public Vector3 realPos;

    void Update()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        RaycastHit hit = new RaycastHit();
        Ray mouse = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(mouse, out RaycastHit hitData))
        {
            realPos = hitData.point;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out hit) && hit.transform.tag == "Start")
            {
                Debug.Log("Start Game");
            }

            if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out hit) && hit.transform.tag == "Quit")
            {
                Debug.Log("Quit Game");
            }
        }
    }
}
