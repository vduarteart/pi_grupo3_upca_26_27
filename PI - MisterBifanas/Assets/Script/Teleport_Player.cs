using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TVStore;

public class Teleport_Player : MonoBehaviour
{
    public GameObject Player;
    public GameObject OutputPlace;

    void OnTriggerEnter(Collider other)
    {
        OutputSocket output = OutputPlace.GetComponent<OutputSocket>();
        
        if (other.CompareTag("Player"))
        {
            if (output.ConnectedInput != null)
            {
                other.GetComponent<CharacterController>().enabled = false;
                Player.transform.position = output.ConnectedInput.transform.position + new Vector3(3f, 0f, 0f);
                other.GetComponent<CharacterController>().enabled = true;
                Debug.Log("Entered TP");
            }
        }
    }
}

/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Teleport_Player : MonoBehaviour
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
        if (Output == choose.OutputLocation)
        {
            Input = choose.InputLocation;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterController>().enabled = false;
            Player.transform.position = Input.transform.position;
            other.GetComponent<CharacterController>().enabled = true;
            Debug.Log("Entered TP");
        }
    }
}*/

