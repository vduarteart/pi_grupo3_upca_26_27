using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Line_Renderer : MonoBehaviour
{
    public GameObject Input_Output;
    public GameObject Camera;
    public GameObject Output;
    public GameObject Input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Choose_In_Out choose = Camera.GetComponent<Choose_In_Out>();
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        Input = choose.InputLocation;

        lineRenderer.SetPosition(0, Output.transform.position);
        lineRenderer.SetPosition(1, Input.transform.position);
    }
}