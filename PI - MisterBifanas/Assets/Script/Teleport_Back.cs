using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Teleport_Back : MonoBehaviour
{
    public GameObject Player;
    public GameObject Output;
    public GameObject Input;
    public GameObject Camera;

    //Get the Output and Input from the other thing
    //Enable the trigger for the tp entrance and exit only when an input and output are linked

    void Update()
    {
        Choose_In_Out choose = Camera.GetComponent<Choose_In_Out>();
        if (Input == choose.InputLocation)
        {
            Output = choose.OutputLocation;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterController>().enabled = false;
            Player.transform.position = Output.transform.position;
            other.GetComponent<CharacterController>().enabled = true;
            Debug.Log("Entered TP");
        }
    }
}