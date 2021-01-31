using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public PlayerControl playerControl;
    public BoardControl boardControl;
    public Ui_Control ui_Control;
    public CameraConrol cameraConrol;
    public AiControl aiControl;

    public Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    public GameObject clickedObject;
    public Dice_Control currentySelected;
    public bool allowInput;

    internal IEnumerator SetupNewGame()//Play game on NewGameScren
    {
        Application.targetFrameRate = 60;

        //reset camera
        //TODO revise to that mainCamera is in CameraControl, and passed into this
        //then have link to load camera zoom when loading.
        //additional, have camera position for each player NB have reset to default button in settings
        mainCamera.transform.position = new Vector3(3, 7, 0);//game start locaion
        mainCamera.transform.rotation = Quaternion.Euler(65f, 355f, 0f);//game start locaion
        mainCamera.transform.rotation.Normalize();

        //reset game components
        boardControl.SetToDefault();
        playerControl.SetToDefault();
        this.SetToDefault();

        //then -> 
        StartCoroutine(KickOffNewGame());
        yield return null;
    }

    internal IEnumerator KickOffNewGame()
    {
        //yield return new WaitForSeconds(.5f);//allow for setup to finish
        playerControl.AddPlayers();
        boardControl.SetupPlayerPieces(playerControl.players);
        playerControl.PrepairePlayerForTurn();
        //ui_Control.SetGameBannerVisable(true);
        AllowInput();
        yield return null;
    }

    void Update()
    {
        if (allowInput && Input.GetMouseButtonDown(0) )
        {
            DisalowInput();
            MouseControl();
        }
    }

    private void MouseControl()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100))
        {
            clickedObject = hit.transform.gameObject;
            if (currentySelected == null)
            {
                if (clickedObject.name == playerControl.activePlayer.name)
                {
                    Dice_Control clickedDice = clickedObject.GetComponent<Dice_Control>();
                    if (clickedDice.lowerDice == false && clickedDice.isBase)
                    {
                        clickedDice = clickedObject.transform.parent.GetComponent<Dice_Control>();
                    }
                    currentySelected = clickedDice;
                    clickedDice.SetSelected();                    
                }
                AllowInput();
            }
            else 
            {
                if (currentySelected.isBase)
                {
                    if (currentySelected.gameObject.GetInstanceID() == clickedObject.GetInstanceID() || 
                        currentySelected.child.gameObject.GetInstanceID() == clickedObject.GetInstanceID())
                    {
                        print("Deselecting currently seleted");
                        currentySelected.SetDeselected();
                        currentySelected = null;
                        AllowInput();
                        return;
                    }

                    float distance = Vector3.Distance(currentySelected.transform.position, clickedObject.transform.position);
                    //print("current distance to clicked object is: " + distance);
                    if (distance <= GlobalVariables.data.BASE_SPAWN_DISTANCE)
                    {
                        if (clickedObject.name == GlobalVariables.data.TILE_NAME || clickedObject.name == currentySelected.player.name)
                        {
                            TileControl clickedTile = clickedObject.GetComponent<TileControl>();
                            if (clickedTile == null) clickedTile = clickedObject.GetComponent<Dice_Control>().tileControl;
                            if (clickedTile.diceOnTile == null)
                            {
                                boardControl.CreateDiceAt(clickedTile, currentySelected.player);
                                playerControl.TakenMove();
                            }
                            else
                            {
                                Dice_Control diceOneTile = clickedTile.diceOnTile;
                                if (currentySelected.player.name == diceOneTile.player.name)
                                {
                                    boardControl.BaseReenforceAdjacent(diceOneTile);
                                } else
                                {
                                    boardControl.CalculateBasesAttackEnemy(currentySelected, diceOneTile);
                                }
                            }
                        }
                        else if (clickedObject.GetComponent<Dice_Control>() != null)
                        {
                            boardControl.CalculateBasesAttackEnemy(currentySelected, clickedObject.GetComponent<Dice_Control>());
                        } else
                        {
                            print("the user clicked a non player, not board object");
                            AllowInput();
                        }
                    } else
                    {
                        TileControl clickedTile = clickedObject.GetComponent<TileControl>();
                        if(clickedTile == null)
                        {
                            Debug.Log($"object too far to consider Creating dice");
                            Dice_Control dice_Control = clickedObject.GetComponent<Dice_Control>();
                            if(dice_Control != null)
                            {
                                boardControl.InvalidMove(dice_Control.tileControl);
                            }                         
                            AllowInput();
                        } else
                        {
                            boardControl.InvalidMove(clickedTile);
                        }
                    }
                } else
                {
                    if (currentySelected.gameObject.GetInstanceID() == clickedObject.GetInstanceID())
                    {
                        print("Deselected currently seleted");
                        currentySelected.SetDeselected();
                        currentySelected = null;
                        AllowInput();
                        return;
                    }                   

                    if (clickedObject.name == GlobalVariables.data.TILE_NAME || clickedObject.name == currentySelected.player.name)
                    {
                        //handles if dice was clicked, get the assosicated tile
                        TileControl clickedTile = clickedObject.GetComponent<TileControl>();
                        if (clickedTile == null) clickedTile = clickedObject.GetComponent<Dice_Control>().tileControl;

                        List<TileControl> path = boardControl.Pathfind(currentySelected.tileControl, clickedTile, playerControl.activePlayer.numberOfMoves);                        
                        if (path == null)
                        {
                            AllowInput();
                            return;
                        }
                        else 
                        {
                            PrintPath(path);
                            if (clickedTile.diceOnTile == null)
                            {
                                boardControl.MoveToEmptyTile(path, currentySelected);
                                AllowInput();

                            }
                            else
                            {
                                Dice_Control diceOneTile = clickedTile.diceOnTile;
                                if (currentySelected.player.name == diceOneTile.player.name)
                                {
                                    //this gets the child dice when a base is the target
                                    if (diceOneTile.isBase && diceOneTile.lowerDice)
                                    {
                                        boardControl.CombineFriendlyDice(currentySelected, diceOneTile.transform.GetChild(6).GetComponent<Dice_Control>(), path);
                                    }
                                    else
                                    {
                                        boardControl.CombineFriendlyDice(currentySelected, diceOneTile, path);
                                    }
                                }
                                else
                                {
                                    if (diceOneTile.isBase)
                                    {
                                        boardControl.CalculateAttackEnemyBase(currentySelected, diceOneTile.transform.GetChild(6).GetComponent<Dice_Control>(), path);
                                    }
                                    else
                                    {
                                        boardControl.CalculateAttackEnemy(currentySelected, diceOneTile, path);
                                    }
                                }
                            }
                        }                        
                    }
                    else
                    {
                        Dice_Control clickedDice = clickedObject.GetComponent<Dice_Control>();
                        if (clickedDice != null)
                        {
                            List<TileControl> path = boardControl.Pathfind(currentySelected.tileControl, clickedDice.tileControl, playerControl.activePlayer.numberOfMoves);
                            if (path == null)
                            {
                                AllowInput();
                                return;
                            }
                            else
                            {
                                PrintPath(path);
                                if (clickedDice.isBase)
                                {
                                    if (clickedDice.lowerDice)
                                    {
                                        boardControl.CalculateAttackEnemyBase(currentySelected, clickedDice.transform.GetChild(6).GetComponent<Dice_Control>(), path);
                                    }
                                    else
                                    {
                                        boardControl.CalculateAttackEnemyBase(currentySelected, clickedDice, path);
                                    }
                                }
                                else
                                {
                                    boardControl.CalculateAttackEnemy(currentySelected, clickedDice, path);
                                }
                            }                            
                        }
                        else
                        {
                            AllowInput();
                            print("the user clicked a non player, not board object");
                        }
                    }
                } 
            }            
        }
        else AllowInput();
    }

    public void AllowInput()
    {
        //Debug.Log("allow input");
        allowInput = true;
    }
    public void DisalowInput()
    {
        //Debug.Log("prevent input");
        allowInput = false;
    }

    private void PrintPath(List<TileControl> path)
    {
        string stringPath = "";
        foreach (TileControl tile in path)
        {
            stringPath += $"{tile.tileIndex}/{tile.pathValue}, ";
        }
        //Debug.Log($"printing out path {path.Count}: {stringPath}");

    }

    public void SetToDefault()
    {
        allowInput = false;
        currentySelected = null;
        clickedObject = null;
    }
}
