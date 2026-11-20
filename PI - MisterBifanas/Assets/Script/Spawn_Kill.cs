using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Spawn_Kill : MonoBehaviour
{
    public GameObject Kill_Player;
    public GameObject Kill_Down;
    public GameObject Kill_Left;
    public GameObject Kill_Bottom;

    public GameObject Spawn_Player;
    public GameObject Spawn_Down;
    public GameObject Spawn_Left;
    public GameObject Spawn_Bottom;

    void Start()
    {
        Reset_The_Scene();
        Debug.Log("Spawned it");
    }

    void Update()
    {
        GameObject player_clone = GameObject.FindGameObjectWithTag("Player");
        Reset_Scene reset_Scene = player_clone.GetComponent<Reset_Scene>();

        bool check = reset_Scene.Reset;

        if (check == true)
        {
            Debug.Log("Called it");
            Reset_The_Scene();
            reset_Scene.Reset = false;
        }
    }

    public void Reset_The_Scene()
    {
        GameObject[] Destruction;
        GameObject[] Players;

        Destruction = GameObject.FindGameObjectsWithTag("Kill");
        Players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var item in  Destruction)
        {
            Destroy(item);
        }

        foreach (var item2 in Players)
        {
            Destroy(item2);
        }

        GameObject Go_Down = Instantiate(Kill_Down, Spawn_Down.transform.position, Spawn_Down.transform.rotation) as GameObject;
        GameObject Go_Left = Instantiate(Kill_Left, Spawn_Left.transform.position, Spawn_Left.transform.rotation) as GameObject;
        GameObject Go_Bottom = Instantiate(Kill_Bottom, Spawn_Bottom.transform.position, Spawn_Bottom.transform.rotation) as GameObject;
        GameObject Go_Player = Instantiate(Kill_Player, Spawn_Player.transform.position, Spawn_Player.transform.rotation) as GameObject;
    }
}
