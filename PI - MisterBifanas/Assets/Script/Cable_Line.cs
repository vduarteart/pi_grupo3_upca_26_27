using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cable_Line : MonoBehaviour
{
    public GameObject[] points;

    public LineRenderer cable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cable = this.gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int array = 0; array < points.Length; array++)
        {
            cable.SetPosition(array, points[array].transform.position);
        }
    }
}
