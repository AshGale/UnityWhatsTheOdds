using UnityEngine;
using TMPro;

public class NewGameScreen : MonoBehaviour
{
    public TMP_InputField name1;
    public TMP_Dropdown colour1;
    public TMP_Dropdown ai1;
    public GameObject included1;
    public GameObject notPlaying1;

    public TMP_InputField name2;
    public TMP_Dropdown colour2;
    public TMP_Dropdown ai2; 
    public GameObject included2;
    public GameObject notPlaying2;

    public TMP_InputField name3;
    public TMP_Dropdown colour3;
    public TMP_Dropdown ai3;
    public GameObject included3;
    public GameObject notPlaying3;

    public TMP_InputField name4;
    public TMP_Dropdown colour4;
    public TMP_Dropdown ai4;
    public GameObject included4;
    public GameObject notPlaying4;

    public void SetDataOnNewGameScreen()//Sets and controles the data for NewGameScreen
    {
        //newGameScreen
        // - could search player prefs for last setup, but doing quickstart

        if (PlayerPrefs.HasKey("cameraSpeed"))
        {
            SetPlayerPreferences();
        }
        else
        {
            NewGameDefault();
        }
    }

    private void SetPlayerPreferences()
    {
        int speed = PlayerPrefs.GetInt("cameraSpeed");
        if (speed != 0)
        {
            GlobalVariables.data.CAMERA_ROTATION_SPEED = speed;
        }
        GlobalVariables.data.SHOW_DICE_INFLATE_ANIMATION = bool.Parse(PlayerPrefs.GetString("diceInflate"));
        name1.text = PlayerPrefs.HasKey("PLAYER1_NAME") ? PlayerPrefs.GetString("PLAYER1_NAME") : GlobalVariables.data.PLAYER1_NAME;
        name2.text = PlayerPrefs.HasKey("PLAYER2_NAME") ? PlayerPrefs.GetString("PLAYER2_NAME") : GlobalVariables.data.PLAYER2_NAME;
        name3.text = PlayerPrefs.HasKey("PLAYER3_NAME") ? PlayerPrefs.GetString("PLAYER3_NAME") : GlobalVariables.data.PLAYER3_NAME;
        name4.text = PlayerPrefs.HasKey("PLAYER4_NAME") ? PlayerPrefs.GetString("PLAYER4_NAME") : GlobalVariables.data.PLAYER4_NAME;
        colour1.value = PlayerPrefs.HasKey("PLAYER1_COLOUR") ? PlayerPrefs.GetInt("PLAYER1_COLOUR") : 0;
        colour2.value = PlayerPrefs.HasKey("PLAYER2_COLOUR") ? PlayerPrefs.GetInt("PLAYER2_COLOUR") : 1;
        colour3.value = PlayerPrefs.HasKey("PLAYER3_COLOUR") ? PlayerPrefs.GetInt("PLAYER3_COLOUR") : 2;
        colour4.value = PlayerPrefs.HasKey("PLAYER4_COLOUR") ? PlayerPrefs.GetInt("PLAYER4_COLOUR") : 3;
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

    internal void SetPlayerPrefs()
    {
        PlayerPrefs.SetString("PLAYER1_NAME", name1.text);
        PlayerPrefs.SetString("PLAYER2_NAME", name2.text);
        PlayerPrefs.SetString("PLAYER3_NAME", name3.text);
        PlayerPrefs.SetString("PLAYER4_NAME", name4.text);
        PlayerPrefs.SetInt("PLAYER1_COLOUR", colour1.value);
        PlayerPrefs.SetInt("PLAYER2_COLOUR", colour2.value);
        PlayerPrefs.SetInt("PLAYER3_COLOUR", colour3.value);
        PlayerPrefs.SetInt("PLAYER4_COLOUR", colour4.value);
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
