using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TVStore;

public class Teleport_Back : MonoBehaviour
{
    public GameObject Player;
    public GameObject Output;
    public GameObject OutputPlace;

    void OnTriggerEnter(Collider other)
    {
        OutputSocket output = OutputPlace.GetComponent<OutputSocket>();

        if (other.CompareTag("Player"))
        {
            if (output.ConnectedInput != null)
            {
                other.GetComponent<CharacterController>().enabled = false;
                Player.transform.position = Output.transform.position + new Vector3(-1f, 0f, 0f);
                other.GetComponent<CharacterController>().enabled = true;
                Debug.Log("Entered TP");
            }
        }
    }
}