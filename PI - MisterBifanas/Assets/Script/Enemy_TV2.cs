using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_TV2 : MonoBehaviour
{
    public GameObject Shadow;
    public GameObject playerPos;
    public float startDelay;
    public float shadowSpeed;

    void Update()
    {
        startDelay -= Time.deltaTime;

        if (startDelay <= 0)
        {
            Shadow.transform.position = Vector3.MoveTowards(Shadow.transform.position, playerPos.transform.position, shadowSpeed * Time.deltaTime);
        }
    }
}