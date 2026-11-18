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

        if (Shadow.transform.position.x > playerPos.transform.position.x)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = -0.23f;
            transform.localScale = localScale;
        }

        if (Shadow.transform.position.x < playerPos.transform.position.x)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = 0.23f;
            transform.localScale = localScale;
        }
    }
}