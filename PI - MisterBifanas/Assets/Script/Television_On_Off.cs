using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Television_On_Off : MonoBehaviour
{
    public GameObject TV2;
    public GameObject TV3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TV2.SetActive(false); 
        TV3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
