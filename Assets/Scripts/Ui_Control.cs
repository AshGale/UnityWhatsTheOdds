using System.Collections;
using TMPro;
using UnityEngine;

public enum ActiveSreen
{
     MainMenu, NewGameMenu, InGameMenu, InGameBanner
}

public class Ui_Control : MonoBehaviour
{
    public GameControl gameControl;
    public NewGameScreen newGameScreen;

    public Material SelectedMaterial;
    public Material DefaultMaterial;
    public TextMeshProUGUI playersMoves;
    public TextMeshProUGUI playersTurn;

    public GameObject mainMenu;
    public GameObject newGameMenu;
    public GameObject gameMenu;
    public GameObject gameBanner;

    // Start is called before the first frame update
    void Start()
    {
        playersMoves.text = GlobalVariables.data.UI_MOVES_TEXT + "0";
        playersTurn.text = GlobalVariables.data.UI_TURN_TEXT + "";
        SetActiveScreens(ActiveSreen.MainMenu);
    }

    //----------------------------------------------------Main Menue Start--------------------------------------------------------------

    public void ContinueButton()
    {
        //need save game functionallity, but could add in 
        SetActiveScreens(ActiveSreen.InGameBanner);
    }

    public void MainMenuButton()
    {
        SetActiveScreens(ActiveSreen.MainMenu);
    }

    public void SettingsButton()
    {

    }

    public void QuitButton()
    {
        Application.Quit();
    }

    //----------------------------------------------------Main Menue End--------------------------------------------------------------
    //----------------------------------------------------In Game Menue Start--------------------------------------------------------------

    public void InGameMenuButton()
    {
        SetActiveScreens(ActiveSreen.InGameMenu);
    }
    
    public void ResumeGameMenuButton()
    {
        SetActiveScreens(ActiveSreen.InGameBanner);
    }

    //----------------------------------------------------In Game Menue End--------------------------------------------------------------
    //----------------------------------------------------New Game Menue Start--------------------------------------------------------------

    public void NewGameMenuButton()
    {
        SetActiveScreens(ActiveSreen.NewGameMenu);
        StartCoroutine(newGameScreen.SetDataOnNewGameScreen());
    }

    public void PlayGameButton()
    {
        //Global Data should be set in new game menue screen
        newGameScreen.SetGlobalData();//apply for use in gameSetup
        SetActiveScreens(ActiveSreen.InGameBanner);
        StartCoroutine(gameControl.SetupNewGame());
    }

    //----------------------------------------------------New Game Menu End--------------------------------------------------------------
    //----------------------------------------------------Game Banner Start--------------------------------------------------------------
    public void UpdateMovesDisplay(int moves)
    {
        playersMoves.text = GlobalVariables.data.UI_MOVES_TEXT + moves;
    }

    public void UpdatePlayersTurnDisplay(string playerName, Material playerMaterial)
    {
        //playersTurn.text = GlobalVariables.data.UI_TURN_TEXT + playerName;
        playersTurn.text = playerName;
        playersTurn.color = playerMaterial.color;

    }
    //----------------------------------------------------Game Banner End--------------------------------------------------------------

    //// Update is called once per frame
    //void Update()
    //{
    //listen for esc butoon
    //}

    public void SetActiveScreens(ActiveSreen activeScreen)
    {
        newGameMenu.SetActive(false);
        gameBanner.SetActive(false);
        gameMenu.SetActive(false);
        mainMenu.SetActive(false);
        gameControl.allowInput = false;

        switch (activeScreen)
        {
            case ActiveSreen.MainMenu:
                {
                    mainMenu.SetActive(true);
                    break;
                }
            case ActiveSreen.NewGameMenu:
                {
                    newGameMenu.SetActive(true);
                    break;
                }
            case ActiveSreen.InGameBanner:
                {
                    gameBanner.SetActive(true);
                    gameControl.allowInput = true;
                    break;
                }
            case ActiveSreen.InGameMenu:
                {
                    gameBanner.SetActive(true);
                    gameMenu.SetActive(true);
                    break;
                }
        }
    }
}
