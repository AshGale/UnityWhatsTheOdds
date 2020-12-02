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
