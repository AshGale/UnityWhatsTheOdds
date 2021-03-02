using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Dice;
    public string playerName;
    public Material playerColour;
    public List<GameObject> diceOwned;
    public Vector3 startLocation;
    public Vector3 cameraPosition;
    public int numberOfMoves;
    public bool is_ai_player = false;
    public AiControl ai;
    public int income;//used for ai

    //TODO add in seperate lists for Outposts, Workers, and Soldiers. will make ai much more performent, at the cost of genearl performace
    // OR add in int to represent ammount of the same, eg 1 outpost, 2 workers, 1 soldier

    public void Init(string playerName, Vector3 startLocation, Material playerColour, Vector3 cameraPosition)
    {
        this.gameObject.name = playerName;
        this.playerName = playerName;
        this.playerColour = playerColour;
        diceOwned = new List<GameObject>();
        this.startLocation = startLocation;
        this.cameraPosition = cameraPosition;
        numberOfMoves = 0;
    }
}
