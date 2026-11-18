using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_TV3 : MonoBehaviour
{
    public GameObject DeathWall;
    public GameObject endPos;
    public float startDelay;
    public float deathSpeed;

    void Update()
    {
        startDelay -= Time.deltaTime;

        if(startDelay <= 0)
        {
            DeathWall.transform.position = Vector3.MoveTowards(DeathWall.transform.position, endPos.transform.position, deathSpeed * Time.deltaTime);
        }
    }
}