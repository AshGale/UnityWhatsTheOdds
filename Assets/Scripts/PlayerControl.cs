using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public GameControl gameControl;
    public GameObject Player;
    public CameraConrol mainCamera;
    public int numberOfPlayers;
    public List<Player> players;
    public Player activePlayer;
    public int indexOfCurrentPlayer;

    public Material blueColour;
    public Material redColour;
    public Material tealColour;
    public Material purpleColour;

    internal IEnumerator KickOffSetup()
    {
        //yield return new WaitForSeconds(.5f);//allow for setup to finish
        numberOfPlayers = GlobalVariables.data.NUMER_OF_PLAYERS;
        yield return null;
    }

    public void AddPlayers()
    {
        print($"Setting {numberOfPlayers} up Players");
        players = new List<Player>();

        //one time search so ok
        NewGameScreen newGameScreen = gameControl.ui_Control.newGameScreen;

        if (newGameScreen.included1.activeSelf)
        {
            GameObject player = Instantiate(Player, new Vector3(0f,0f,0f), Quaternion.identity);
            player.transform.SetParent(this.transform);
            player.GetComponent<Player>().Init(GlobalVariables.data.PLAYER1_NAME,
                GlobalVariables.data.PLAYER_1_START,
                GetPlayerColour(GlobalVariables.data.PLAYER1_COLOUR),
                GlobalVariables.data.PLAYER_1_CAMERA_DEFAULT);
            players.Add(player.GetComponent<Player>());
        }
        if (newGameScreen.included2.activeSelf)
        {
            GameObject player = Instantiate(Player, new Vector3(0f, 0f, 0f), Quaternion.identity);
            player.transform.SetParent(this.transform);
            player.GetComponent<Player>().Init(GlobalVariables.data.PLAYER2_NAME,
                GlobalVariables.data.PLAYER_2_START,
                GetPlayerColour(GlobalVariables.data.PLAYER2_COLOUR),
                GlobalVariables.data.PLAYER_2_CAMERA_DEFAULT);
            players.Add(player.GetComponent<Player>());
        }
        if (newGameScreen.included3.activeSelf)
        {
            GameObject player = Instantiate(Player, new Vector3(0f, 0f, 0f), Quaternion.identity);
            player.transform.SetParent(this.transform);
            player.GetComponent<Player>().Init(GlobalVariables.data.PLAYER3_NAME,
                GlobalVariables.data.PLAYER_3_START,
                GetPlayerColour(GlobalVariables.data.PLAYER3_COLOUR),
                GlobalVariables.data.PLAYER_3_CAMERA_DEFAULT);
            players.Add(player.GetComponent<Player>());
        }
        if (newGameScreen.included4.activeSelf)
        {
            GameObject player = Instantiate(Player, new Vector3(0f, 0f, 0f), Quaternion.identity);
            player.transform.SetParent(this.transform);
            player.GetComponent<Player>().Init(GlobalVariables.data.PLAYER4_NAME,
                GlobalVariables.data.PLAYER_4_START,
                GetPlayerColour(GlobalVariables.data.PLAYER4_COLOUR),
                GlobalVariables.data.PLAYER_4_CAMERA_DEFAULT);
            players.Add(player.GetComponent<Player>());
        }
        indexOfCurrentPlayer = 0;
        activePlayer = players.ElementAt(indexOfCurrentPlayer);
        
    }

    internal void TakenMove(int moveValue = 1)
    {
        //gameControl.AllowInput();
        activePlayer.numberOfMoves -= moveValue;
        if (activePlayer.numberOfMoves <= 0)
            NextPlayer();
        gameControl.ui_Control.UpdateMovesDisplay(activePlayer.numberOfMoves);
    }

    public void NextPlayer ()
    {
        indexOfCurrentPlayer++;
        if(indexOfCurrentPlayer >= players.Count)
        {
            indexOfCurrentPlayer = 0;
        }
        activePlayer = players.ElementAt(indexOfCurrentPlayer);
        Debug.Log($"it is now {activePlayer.name}'s turn of {players.Count} players");
        
        if(gameControl.currentySelected != null)
        {
            gameControl.currentySelected.SetDeselected();
            gameControl.currentySelected = null;
        }
            
        GenerateIncomeForPlayer();
    }

    public void GenerateIncomeForPlayer()
    {
        int dice_value;
        Dice_Control dice_Control;
        foreach (GameObject dice in activePlayer.diceOwned)
        {
            dice_Control = dice.GetComponent<Dice_Control>();
            dice_value = dice_Control.value;
            if(dice_Control.isBase)
            {
                activePlayer.numberOfMoves += GlobalVariables.data.MOVES_FOR_BASE;
            } else if (dice_value % 2 == 1)
            {
                switch (dice_value)
                {
                    case 1: activePlayer.numberOfMoves += GlobalVariables.data.INCOME_1; break;
                    case 3: activePlayer.numberOfMoves += GlobalVariables.data.INCOME_3; break;
                    case 5: activePlayer.numberOfMoves += GlobalVariables.data.INCOME_5; break;
                }
            }
        }

        if (activePlayer.numberOfMoves == 0)
        {
            Debug.Log($"player {activePlayer.name} has no moves, and is out of the game!");
            RemovePlayer(activePlayer);
            if (players.Count > 1)
            {
                activePlayer = players.ElementAt(indexOfCurrentPlayer);
                Debug.Log($"it is now {activePlayer.name}'s turn of {players.Count} players");
                GenerateIncomeForPlayer();
            }
            else
            {
                Debug.Log($"setting to last player!");
                NextPlayer();
                Debug.Log($"Only player {players[0].name} remains. \n Game Over, well done!");
                gameControl.ui_Control.SetActiveScreens(ActiveSreen.MainMenu);
            }
        }
        else
        {
            gameControl.ui_Control.UpdatePlayersTurnDisplay(activePlayer.playerName, activePlayer.playerColour);               
            gameControl.ui_Control.UpdateMovesDisplay(activePlayer.numberOfMoves);
            StartCoroutine(mainCamera.GlidePosition(activePlayer.cameraPosition));
            foreach (GameObject dice in activePlayer.diceOwned)
            {
                dice_Control = dice.GetComponent<Dice_Control>();
                dice_value = dice_Control.value;
                if (dice_Control.isBase)
                {
                    Dice_Control childDice = dice_Control.transform.GetChild(6).GetComponent<Dice_Control>();
                    StartCoroutine(childDice.FlashForNewTurn());
                }
                StartCoroutine(dice_Control.FlashForNewTurn());
            }
        }
    }

    public void RemovePlayer(Player playerToRemove)
    {
        Dice_Control dice_Control;
        GameObject diceObject;
        bool playerHasDice = playerToRemove.diceOwned.Count > 0;
        while (playerHasDice)
        {
            diceObject = playerToRemove.diceOwned[0];
            //Debug.Log($"Reming dice {diceObject.GetInstanceID()} of {playerToRemove.diceOwned.Count}");
            dice_Control = diceObject.GetComponent<Dice_Control>();
            if (dice_Control.isBase)
            {
                Debug.Log($"GameMode base generate 0, destroyting Base");
                gameControl.boardControl.DeconstrucetBase(dice_Control.transform.GetChild(6).GetComponent<Dice_Control>(), 6);
            }
            dice_Control.StopAnimation();
            gameControl.boardControl.DestroySingleDice(dice_Control);

            playerHasDice = playerToRemove.diceOwned.Count > 0;
        }
        Debug.Log($"Removed player {playerToRemove.name}");
        players.Remove(playerToRemove);
    }

    private Material GetPlayerColour(string colour)
    {
        Debug.Log("colour" + GlobalVariables.data.PLAYER1_COLOUR);

        //This is synced with Ui.dropdown on newgame screen
        switch (colour) {
            case "Blue":  return blueColour;           
            case "Red": return redColour;
            case "Teal": return tealColour;
            case "Purple": return purpleColour;
            default: return null;
        }
    }

    public void SetToDefault()
    {
        //remove player objects and dice
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(KickOffSetup());
    }
}
