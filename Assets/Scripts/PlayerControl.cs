using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public GameControl gameControl;

    public Material blueColour;
    public Material redColour;
    public Material tealColour;
    public Material purpleColour;

    public GameObject Player;
    public int numberOfPlayers;
    public List<Player> players;
    public Player activePlayer;
    public int indexOfCurrentPlayer;

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

            //for testing
            player.GetComponent<Player>().ai = gameControl.aiControl;



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


    //-------------------------------------------------------------------Helper functions for ai
    public List<Dice_Control> GetAllPlayerWorkers(Player player)
    {
        List<Dice_Control> workers = new List<Dice_Control>();
        foreach (Dice_Control dice in player.GetComponentsInChildren<Dice_Control>())
        {
            //is dice value odd ie worker
            if (dice.value % 2 == 1 && !dice.isBase)
            {
                workers.Add(dice);
            }
        }
        return workers;
    }

    public Dice_Control GetFirstPlayerWorker(Player player)
    {
        foreach (Dice_Control dice in player.GetComponentsInChildren<Dice_Control>())
        {
            //is dice value odd ie worker
            if (dice.value % 2 == 1 && !dice.isBase)
            {
                return dice;
            }
        }
        return null;
    }

    public List<Dice_Control> GetAllPlayerSoldiers(Player player)
    {
        List<Dice_Control> soldiers = new List<Dice_Control>();
        foreach (Dice_Control dice in player.GetComponentsInChildren<Dice_Control>())
        {
            //is dice value odd ie worker
            if (dice.value % 2 == 0 && !dice.isBase)
            {
                soldiers.Add(dice);
            }
        }
        return soldiers;
    }

    public Dice_Control GetFirstPlayerSoldier(Player player)
    {
        foreach (Dice_Control dice in player.GetComponentsInChildren<Dice_Control>())
        {
            //is dice value even ie Soldier
            if (dice.value % 2 == 0 && !dice.isBase)
            {
                return dice;
            }
        }
        return null;
    }

    public Dice_Control GetFirstPlayerOutpost(Player player)
    {
        foreach (Dice_Control dice in player.GetComponentsInChildren<Dice_Control>())
        {
            //is dice value even ie Soldier
            if (dice.isBase)
            {
                return dice;
            }
        }
        return null;
    }
    //-------------------------------------------------------------------

    public int GenerateIncomeForPlayer(List<GameObject> playersDiceOwned)
    {
        int dice_value;
        Dice_Control dice_Control;
        int moveForPlayer = 0;

        foreach (GameObject dice in playersDiceOwned)
        {
            dice_Control = dice.GetComponent<Dice_Control>();
            dice_value = dice_Control.value;
            if (dice_Control.isBase)
            {
                moveForPlayer += GlobalVariables.data.MOVES_FOR_BASE;
            }
            else if (dice_value % 2 == 1)
            {
                switch (dice_value)
                {
                    case 1: moveForPlayer += GlobalVariables.data.WORKER_INCOME_1; break;
                    case 3: moveForPlayer += GlobalVariables.data.WORKER_INCOME_3; break;
                    case 5: moveForPlayer += GlobalVariables.data.WORKER_INCOME_5; break;
                }
            }
        }
        return moveForPlayer;
    }

    public void TakeMovesFromPlayer(Player targetPlayer, int moveValue = 1)
    {
        targetPlayer.numberOfMoves -= moveValue;
        gameControl.ui_Control.UpdateMovesDisplay(targetPlayer.numberOfMoves);
    }

    internal void TakenMove(int moveValue = 1)
    {
        activePlayer.numberOfMoves -= moveValue;
        if (activePlayer.numberOfMoves <= 0)
        {
            gameControl.DisalowInput();
            if(gameControl.currentySelected != null)
            {
                gameControl.currentySelected.SetDeselected();
                gameControl.currentySelected = null;
            }
            NextPlayer();
        }
        else
        {
            gameControl.ui_Control.UpdateMovesDisplay(activePlayer.numberOfMoves);
        }
    }

    public void IncrementCurrentPlayer()
    {
        indexOfCurrentPlayer++;
        if (indexOfCurrentPlayer >= players.Count)
        {
            indexOfCurrentPlayer = 0;
        }
        activePlayer = players.ElementAt(indexOfCurrentPlayer);
        
    }

    public void NextPlayer ()
    {

        if (gameControl.currentySelected != null)
        {
            gameControl.currentySelected.SetDeselected();
            gameControl.currentySelected = null;
        }

        IncrementCurrentPlayer();
        PrepairePlayerForTurn();
    }

    public void PrepairePlayerForTurn()
    {
        int actionsForPlayer = GenerateIncomeForPlayer(activePlayer.diceOwned);

        if (actionsForPlayer == 0)
        {
            EliminatePlayer();
        }
        else
        {
            Debug.Log($"it is now {activePlayer.name}'s turn, with {actionsForPlayer} actions");
            activePlayer.numberOfMoves = actionsForPlayer;
            StartPlayerTurn();
        }
    }

    public void StartPlayerTurn()
    {
        int dice_value;
        Dice_Control dice_Control;
        gameControl.ui_Control.UpdatePlayersTurnDisplay(activePlayer.playerName, activePlayer.playerColour);
        gameControl.ui_Control.UpdateMovesDisplay(activePlayer.numberOfMoves);
        StartCoroutine(gameControl.cameraConrol.GlidePosition(activePlayer.cameraPosition));

        if (GlobalVariables.data.SHOW_FLASH_START_TURN)
        {
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

        if (activePlayer.ai != null)
        {
            AiPlayersTurn(activePlayer);
        } else
        {
            gameControl.AllowInput();
        }
    }

    public void AiPlayersTurn(Player player)
    {
        gameControl.aiControl.EasyAi(player);
    }

    public void EliminatePlayer()
    {
        Debug.Log($"player {activePlayer.name} has no moves, and is eliminated from the game!");
        RemovePlayer(activePlayer);
        if (players.Count > 1)
        {
            activePlayer = players.ElementAt(indexOfCurrentPlayer);
            PrepairePlayerForTurn();
        }
        else
        {
            ShowGameOver();
        }
    }

    public void ShowGameOver()
    {
        //have a game over screen. new game or continue
        Debug.Log($"setting to last player!");
        NextPlayer();
        Debug.Log($"Only the team {players[0].name} remains. \n Game Over, well done!");
        gameControl.ui_Control.SetActiveScreens(ActiveSreen.MainMenu);
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
