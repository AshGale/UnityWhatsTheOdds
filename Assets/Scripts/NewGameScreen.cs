using System.Collections;
using UnityEngine;
using TMPro;

public class NewGameScreen : MonoBehaviour
{

    public TMP_InputField name1;
    public TMP_Dropdown colour1;
    public GameObject included1;
    public GameObject notPlaying1;

    public TMP_InputField name2;
    public TMP_Dropdown colour2;
    public GameObject included2;
    public GameObject notPlaying2;

    public TMP_InputField name3;
    public TMP_Dropdown colour3;
    public GameObject included3;
    public GameObject notPlaying3;

    public TMP_InputField name4;
    public TMP_Dropdown colour4;
    public GameObject included4;
    public GameObject notPlaying4;

    internal IEnumerator SetDataOnNewGameScreen()//Sets and controles the data for NewGameScreen
    {
        //newGameScreen
        // - could search player prefs for last setup, but doing quickstart

        NewGameDefault();

        yield return null;
    }

    private void NewGameDefault()
    {
        GlobalVariables data = GlobalVariables.data;

        included1.SetActive(true);
        notPlaying1.SetActive(false);
        name1.text = data.PLAYER1_NAME;
        colour1.value = 0;

        included2.SetActive(true);
        notPlaying2.SetActive(false);
        name2.text = data.PLAYER2_NAME;
        colour2.value = 1;

        included3.SetActive(false);
        notPlaying3.SetActive(true);
        name3.text = data.PLAYER3_NAME;
        colour3.value = 2;

        included4.SetActive(false);
        notPlaying4.SetActive(true);
        name4.text = data.PLAYER4_NAME;
        colour4.value = 3;
    }

    public void SetGlobalData()
    {
        //check how many player there are
        GlobalVariables.data.NUMER_OF_PLAYERS = DeterminNumberOfPlayers();

        //here is here you actually reference global data, and set the fields
        GlobalVariables.data.PLAYER1_NAME = name1.text;
        GlobalVariables.data.PLAYER2_NAME = name2.text;
        GlobalVariables.data.PLAYER3_NAME = name3.text;
        GlobalVariables.data.PLAYER4_NAME = name4.text;

        GlobalVariables.data.PLAYER1_COLOUR = colour1.options[colour1.value].text;
        GlobalVariables.data.PLAYER2_COLOUR = colour2.options[colour2.value].text;
        GlobalVariables.data.PLAYER3_COLOUR = colour3.options[colour3.value].text;
        GlobalVariables.data.PLAYER4_COLOUR = colour4.options[colour4.value].text;
    }

    public void RemovePlayer1()
    {
        included1.SetActive(false);
        notPlaying1.SetActive(true);
        GlobalVariables.data.NUMER_OF_PLAYERS--;
    }

    public void RemovePlayer2()
    {
        included2.SetActive(false);
        notPlaying2.SetActive(true);
        GlobalVariables.data.NUMER_OF_PLAYERS--;
    }

    public void RemovePlayer3()
    {
        included3.SetActive(false);
        notPlaying3.SetActive(true);
        GlobalVariables.data.NUMER_OF_PLAYERS--;
    }

    public void RemovePlayer4()
    {
        included4.SetActive(false);
        notPlaying4.SetActive(true);
        GlobalVariables.data.NUMER_OF_PLAYERS--;
    }

    public void AddPlayer1()
    {
        included1.SetActive(true);
        notPlaying1.SetActive(false);
        GlobalVariables.data.NUMER_OF_PLAYERS++;
    }

    public void AddPlayer2()
    {
        included2.SetActive(true);
        notPlaying2.SetActive(false);
        GlobalVariables.data.NUMER_OF_PLAYERS++;
    }

    public void AddPlayer3()
    {
        included3.SetActive(true);
        notPlaying3.SetActive(false);
        GlobalVariables.data.NUMER_OF_PLAYERS++;
    }

    public void AddPlayer4()
    {
        included4.SetActive(true);
        notPlaying4.SetActive(false);
        GlobalVariables.data.NUMER_OF_PLAYERS++;
    }

    private int DeterminNumberOfPlayers()
    {
        int numberOfPlayers = 0;
        if (included1.activeSelf)
        {
            numberOfPlayers++;
        }
        if (included2.activeSelf)
        {
            numberOfPlayers++;
        }
        if (included3.activeSelf)
        {
            numberOfPlayers++;
        }
        if (included4.activeSelf)
        {
            numberOfPlayers++;
        }
        return numberOfPlayers;
    }

}
