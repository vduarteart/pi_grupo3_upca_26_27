using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Move_Cable : MonoBehaviour
{
    public GameObject OutputLocation;
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
            if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out hit) && hit.transform.tag == "Output")
            {
                OutputLocation = hit.transform.gameObject;
                OutputLocation.transform.position = realPos;
                Debug.Log("Hit Output");
            }
        }
    }
}
