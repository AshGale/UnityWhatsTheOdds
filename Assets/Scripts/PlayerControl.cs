using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

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

            Player player1 = player.GetComponent<Player>();

            player1.Init(GlobalVariables.data.PLAYER1_NAME,
                GlobalVariables.data.PLAYER_1_START,
                GetPlayerColour(GlobalVariables.data.PLAYER1_COLOUR),
                GlobalVariables.data.PLAYER_1_CAMERA_DEFAULT);

            newGameScreen.ai1.value = 3;
            ApplyAiIfSet(newGameScreen.ai1, player1);

            players.Add(player1);
        }
        if (newGameScreen.included2.activeSelf)
        {
            GameObject player = Instantiate(Player, new Vector3(0f, 0f, 0f), Quaternion.identity);
            player.transform.SetParent(this.transform);

            Player player2 = player.GetComponent<Player>();

            player2.Init(GlobalVariables.data.PLAYER2_NAME,
                GlobalVariables.data.PLAYER_2_START,
                GetPlayerColour(GlobalVariables.data.PLAYER2_COLOUR),
                GlobalVariables.data.PLAYER_2_CAMERA_DEFAULT);

            //for testing
            newGameScreen.ai2.value = 1;
            ApplyAiIfSet(newGameScreen.ai2, player2);

            players.Add(player2);

        }
        if (newGameScreen.included3.activeSelf)
        {
            GameObject player = Instantiate(Player, new Vector3(0f, 0f, 0f), Quaternion.identity);
            player.transform.SetParent(this.transform);

            Player player3 = player.GetComponent<Player>();

            player3.Init(GlobalVariables.data.PLAYER3_NAME,
                GlobalVariables.data.PLAYER_3_START,
                GetPlayerColour(GlobalVariables.data.PLAYER3_COLOUR),
                GlobalVariables.data.PLAYER_3_CAMERA_DEFAULT);

            //for testing
            newGameScreen.ai3.value = 2;
            ApplyAiIfSet(newGameScreen.ai3, player3);

            players.Add(player3);
        }
        if (newGameScreen.included4.activeSelf)
        {
            GameObject player = Instantiate(Player, new Vector3(0f, 0f, 0f), Quaternion.identity);
            player.transform.SetParent(this.transform);

            Player player4 = player.GetComponent<Player>();

            player4.Init(GlobalVariables.data.PLAYER4_NAME,
                GlobalVariables.data.PLAYER_4_START,
                GetPlayerColour(GlobalVariables.data.PLAYER4_COLOUR),
                GlobalVariables.data.PLAYER_4_CAMERA_DEFAULT);

            //for testing
            newGameScreen.ai4.value = 3;
            ApplyAiIfSet(newGameScreen.ai4, player4);

            players.Add(player4);
        }
        indexOfCurrentPlayer = 0;
        activePlayer = players.ElementAt(indexOfCurrentPlayer);        
    }


    //-------------------------------------------------------------------Helper functions for ai

    void ApplyAiIfSet(TMP_Dropdown aiType, Player player)
    {
        if (aiType.options[aiType.value].text != AiDifficulty.Player.ToString())
        {
            GameObject ai = Instantiate(gameControl.aiControl, new Vector3(0f, 0f, 0f), Quaternion.identity);
            ai.transform.SetParent(gameControl.aiControl.transform);

            player.ai = ai.GetComponent<AiControl>();
            player.is_ai_player = true;

            switch (aiType.value)
            {
                case 0: player.ai.SetDifficulty(AiDifficulty.Player); break;
                case 1: player.ai.SetDifficulty(AiDifficulty.Easy); break;
                case 2: player.ai.SetDifficulty(AiDifficulty.Medium); break;
                case 3: player.ai.SetDifficulty(AiDifficulty.Hard); break;
            }
        }
    }

    public List<Dice_Control> GetAllPlayerWorkers(Player player)
    {
        return GetWorkersFromList(player.GetComponentsInChildren<Dice_Control>());
    }

    public Dice_Control GetFirstPlayerWorker(Player player)
    {
        List<Dice_Control> workers = GetWorkersFromList(player.GetComponentsInChildren<Dice_Control>());
        return workers.Count <= 0 ? null : workers[0]; 
    }

    public List<Dice_Control> GetWorkersFromList(Dice_Control[] dice_Controls)
    {
        List<Dice_Control> workers = new List<Dice_Control>();
        foreach (Dice_Control dice in dice_Controls)
        {
            //is dice value odd ie worker
            if (dice.currentValue % 2 == 1 && !dice.isBase)
            {
                workers.Add(dice);
            }
        }
        return workers;
    }

    public List<Dice_Control> GetAllPlayerOutpost(Player player)
    {
        return GetOutpostsFromList(player.GetComponentsInChildren<Dice_Control>());
    }

    public Dice_Control GetFirstPlayerOutpost(Player player)
    {
        List<Dice_Control> outposts = GetOutpostsFromList(player.GetComponentsInChildren<Dice_Control>());
        return outposts.Count <= 0 ? null : outposts[0];
    }

    public List<Dice_Control> GetOutpostsFromList(Dice_Control[] dice_Controls)
    {
        List<Dice_Control> outposts = new List<Dice_Control>();
        foreach (Dice_Control dice in dice_Controls)
        {
            //is dice value odd ie worker
            if (dice.isBase && dice.lowerDice == false)
            {
                outposts.Add(dice);
            }
        }
        return outposts;
    }

    public List<Dice_Control> GetAllPlayerSoldiers(Player player)
    {
        return GetSoldiersFromList(player.GetComponentsInChildren<Dice_Control>());
    }

    public Dice_Control GetFirstPlayerSoldier(Player player)
    {
        List<Dice_Control> soldiers = GetSoldiersFromList(player.GetComponentsInChildren<Dice_Control>());
        return soldiers.Count <= 0 ? null : soldiers[0];
    }

    public List<Dice_Control> GetSoldiersFromList(Dice_Control[] dice_Controls)
    {
        List<Dice_Control> soldiers = new List<Dice_Control>();
        foreach (Dice_Control dice in dice_Controls)
        {
            //is dice value odd ie worker
            if (dice.currentValue % 2 == 0 && !dice.isBase)
            {
                soldiers.Add(dice);
            }
        }
        return soldiers;
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
            dice_value = dice_Control.currentValue;
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

    internal void TakenMove(int moveValue = 1)
    {
        activePlayer.income = GenerateIncomeForPlayer(activePlayer.diceOwned);
        Debug.Log($"Taking {moveValue} form {activePlayer.name}, now at {activePlayer.numberOfMoves - moveValue}, with +{activePlayer.income}");
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
            
            gameControl.ui_Control.UpdateMovesDisplay(activePlayer.numberOfMoves, activePlayer.income);
            if (activePlayer.ai == null)
            {
                gameControl.AllowInput();
            }
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
            activePlayer.income = actionsForPlayer;
            StartPlayerTurn(actionsForPlayer);
        }
    }

    public void StartPlayerTurn(int actionsForPlayer)
    {
        int dice_value;
        Dice_Control dice_Control;
        gameControl.ui_Control.UpdatePlayersTurnDisplay(activePlayer.playerName, activePlayer.playerColour);
        gameControl.ui_Control.UpdateMovesDisplay(activePlayer.numberOfMoves, actionsForPlayer);
        StartCoroutine(gameControl.cameraConrol.GlidePosition(activePlayer.cameraPosition));

        if (GlobalVariables.data.SHOW_FLASH_START_TURN && activePlayer.ai == null)
        {
            foreach (GameObject dice in activePlayer.diceOwned)
            {
                dice_Control = dice.GetComponent<Dice_Control>();
                dice_value = dice_Control.currentValue;
                if (dice_Control.isBase)
                {
                    Dice_Control childDice = dice_Control.transform.GetChild(6).GetComponent<Dice_Control>();
                    StartCoroutine(childDice.FlashForNewTurn());
                }
                StartCoroutine(dice_Control.FlashForNewTurn());
            }
        }

        if (activePlayer.is_ai_player)
        {
            AiPlayersTurn(activePlayer);
        } else
        {
            gameControl.AllowInput();
        }
    }

    public void AiPlayersTurn(Player player)
    {
        player.ai.DoTurn(player);//revise, as upstaed workflow
    }

    public void EliminatePlayer()
    {
        Debug.Log($"player {activePlayer.name} has no moves, and is eliminated from the game!");
        RemovePlayer(activePlayer);
        if (players.Count > 1)
        {
            if (indexOfCurrentPlayer >= players.Count)
            {
                indexOfCurrentPlayer = 0;
            }
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
