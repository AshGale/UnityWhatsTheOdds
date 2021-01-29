using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialPage
{
    Popup, OverView, Pieces, Outposts, Soldiers, Workers, WinCondition, LooseCondition, IlligalMoves, Extra
}

public class TutorialControl : MonoBehaviour
{

    public GameControl gameControl;

    public GameObject popup;
    public GameObject overview;
    public GameObject pieces;
    public GameObject outposts;
    public GameObject soldiers;
    public GameObject workers;
    public GameObject winConditions;
    public GameObject looseConditions;
    public GameObject illigalMoves;
    public GameObject extra;

    public void StartTutorial()
    {
        ShowOverView();
    }

    public void ShowPopup()
    {
        SetTutorialPage(TutorialPage.Popup);
    }

    public void ShowOverView()
    {
        SetTutorialPage(TutorialPage.OverView);
    }

    public void ShowPieces()
    {
        SetTutorialPage(TutorialPage.Pieces);
    }

    public void ShowOutposts()
    {
        SetTutorialPage(TutorialPage.Outposts);
    }

    public void ShowSoldiers()
    {
        SetTutorialPage(TutorialPage.Soldiers);
    }

    public void ShowWorkers()
    {
        SetTutorialPage(TutorialPage.Workers);
    }

    public void ShowWinConditions()
    {
        SetTutorialPage(TutorialPage.WinCondition);
    }

    public void ShowLooseConditions()
    {
        SetTutorialPage(TutorialPage.LooseCondition);
    }

    public void ShowIlligalMoves()
    {
        SetTutorialPage(TutorialPage.IlligalMoves);
    }


    public void ShowExtra()
    {
        SetTutorialPage(TutorialPage.Extra);
    }

    public void SetTutorialPage(TutorialPage page)
    {
        popup.SetActive(false);
        overview.SetActive(false);
        pieces.SetActive(false);
        outposts.SetActive(false);
        soldiers.SetActive(false);
        workers.SetActive(false);
        winConditions.SetActive(false);
        looseConditions.SetActive(false);
        illigalMoves.SetActive(false);
        extra.SetActive(false);
        //gameControl.allowInput = false;

        switch (page)
        {
            case TutorialPage.Popup:
                {
                    popup.SetActive(true);
                    break;
                }
            case TutorialPage.OverView:
                {
                    overview.SetActive(true);
                    break;
                }
            case TutorialPage.Pieces:
                {
                    pieces.SetActive(true);
                    break;
                }
           case TutorialPage.Outposts:
                {
                    outposts.SetActive(true);
                    break;
                }
            case TutorialPage.Soldiers:
                {
                    soldiers.SetActive(true);
                    break;
                }
            case TutorialPage.Workers:
                {
                    workers.SetActive(true);
                    break;
                }
            case TutorialPage.WinCondition:
                {
                    winConditions.SetActive(true);
                    break;
                }
            case TutorialPage.LooseCondition:
                {
                    looseConditions.SetActive(true);
                    break;
                }
            case TutorialPage.IlligalMoves:
                {
                    illigalMoves.SetActive(true);
                    break;
                }
            case TutorialPage.Extra:
                {
                    extra.SetActive(true);
                    break;
                }
        }
    }
}
