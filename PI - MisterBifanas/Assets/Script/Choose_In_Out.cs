using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Choose_In_Out : MonoBehaviour
{
    public GameObject InputLocation;
    public GameObject OutputLocation;
    public bool cable = false;
    public Vector3 realPos;

    void Start()
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.green;

        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        RaycastHit hit = new RaycastHit();
        Ray mouse = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(mouse, out RaycastHit hitData))
        {
            realPos = hitData.point;
        }

        if (Mouse.current.leftButton.isPressed)
        {

            if (cable == false)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out hit) && hit.transform.tag == "Output")
                {
                    OutputLocation = hit.transform.gameObject;
                    lineRenderer.SetPosition(0, OutputLocation.transform.position);
                    OutputLocation.SetActive(true);
                    print("Output found");
                    cable = true;
                }
            }

            if (cable == true)
            {
                lineRenderer.SetPosition(1, realPos);

                if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out hit) && hit.transform.tag == "Input")
                {
                    InputLocation = hit.transform.gameObject;
                    InputLocation.SetActive(true);
                    print("Input found");
                    cable = false;
                }
            }

        }
    }
}