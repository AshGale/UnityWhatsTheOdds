using UnityEngine;
using TMPro;

public enum ActiveSreen
{
     MainMenu, NewGameMenu, InGameMenu, InGameBanner, SettingsMenu, Tutorial
}

public class Ui_Control : MonoBehaviour
{
    public GameControl gameControl;
    public NewGameScreen newGameScreen;
    public SettingsScreen settingsScreen;
    public TutorialControl tutorialControl;

    public Material SelectedMaterial;
    public Material DefaultMaterial;
    public TextMeshProUGUI playerActionsText;
    public TextMeshProUGUI teamNameText;

    public GameObject mainMenu;
    public GameObject newGameMenu;
    public GameObject gameMenu;
    public GameObject gameBanner;
    public GameObject settings;
    public GameObject tutorial;

    // Start is called before the first frame update
    void Start()
    {
        playerActionsText.text = GlobalVariables.data.UI_ACTIONS_TEXT + "0";
        teamNameText.text = GlobalVariables.data.UI_TURN_TEXT + "";
        SetActiveScreens(ActiveSreen.MainMenu);
    }

    //----------------------------------------------------Main Menue Start--------------------------------------------------------------

    public void ContinueButton()
    {
        if (gameControl.playerControl.players.Count > 0)
        {
            //need save game functionallity, but could add in 
            SetActiveScreens(ActiveSreen.InGameBanner);
        }
            
    }

    public void MainMenuButton()
    {
        SetActiveScreens(ActiveSreen.MainMenu);
    }

    public void ShowTutorialPopup()
    {
        SetActiveScreens(ActiveSreen.Tutorial);
        tutorialControl.ShowPopup();
    }

    public void SettingsButton()
    {
        settingsScreen.LoadSceen();
        SetActiveScreens(ActiveSreen.SettingsMenu);
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
        newGameScreen.SetDataOnNewGameScreen();
    }

    public void PlayGameButton()
    {
        //Global Data should be set in new game menue screen
        newGameScreen.SetGlobalData();//apply for use in gameSetup
        newGameScreen.SetPlayerPrefs();
        SetActiveScreens(ActiveSreen.InGameBanner);
        StartCoroutine(gameControl.SetupNewGame());
    }

    //----------------------------------------------------New Game Menu End--------------------------------------------------------------
    //----------------------------------------------------Game Banner Start--------------------------------------------------------------
    public void UpdateMovesDisplay(int moves)
    {
        playerActionsText.text = GlobalVariables.data.UI_ACTIONS_TEXT + moves;
    }

    public void UpdatePlayersTurnDisplay(string playerName, Material playerMaterial)
    {
        //playersTurn.text = GlobalVariables.data.UI_TURN_TEXT + playerName;
        teamNameText.text = playerName;
        teamNameText.color = playerMaterial.color;

    }
    //----------------------------------------------------Game Banner End--------------------------------------------------------------
    //----------------------------------------------------Tutorial Start--------------------------------------------------------------

    public void LaunchTutorial()
    {
        //This should probably be a scene
        //SetActiveScreens(ActiveSreen.InGameBanner);
        //SetActiveScreens(ActiveSreen.Tutorial);
        tutorialControl.StartTutorial();
        //
    }

    public void SkipTutorial()
    {
        SetActiveScreens(ActiveSreen.MainMenu);
        //This should probably be a scene

        //launch set of text boxes with explination
        /*
         * Game overview
         *      Turn base statigy board game, where last player standing wins.
         *      each turn use your actions to inprove your strength for the next round.
         *      combine dice together, to make stronger unites, and earn as much actions per turns as possable
         *      cripple the other players by attacking there outposts, and restrict player acions by reducing workings
         * describe pieces
         *  Outposts
         *      start the game with one, is 12 in value becomes a value 6 soldier when killed
         *      can create new pieces adjacent to the outpost, ie in all four directions
         *      can add value to adjact dice up to a value of 6
         *      can attack enemy units
         *  workers 
         *      generate actions per turn based on their value
         *      1 generates 1 action
         *      3 generates 2 actions
         *      5 generates 3 actions
         *  soldier
         *      can attack other enemy pieces
         *      this pice is essential to 
         *      value 6 soldiers, can move onto another 6, and create a value 12 outpost
         * win conditions
         *      knock out all player, or players have no moves at the start of their turn
         * loose condition
         *      you start the turn with 0 actions eg you have 3 units with value 4 4 6 will give 0 actions next round
         * illigal actions
         *      eg outposts generate units move than 1 square away
         *      moving a value 3 unit onto a 6
         * Bonus tips
         *      you can move a 6 into a 2 and it will become a 6, leaving a 2 behind
         *      
         */
    }

    //----------------------------------------------------Tutorial End--------------------------------------------------------------

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
        settings.SetActive(false);
        tutorial.SetActive(false);
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
            case ActiveSreen.SettingsMenu:
                {
                    settings.SetActive(true);
                    break;
                }
            case ActiveSreen.Tutorial:
                {
                    tutorial.SetActive(true);
                    break;
                }
        }
    }
}
