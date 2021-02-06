using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiControl : MonoBehaviour
{

    public GameControl gameControl;
    public PathFinding pathFinding;

    public Dice_Control target;
    public Dice_Control leiginToUse;

    public async void EasyAi(Player player)
    {
        Debug.Log($"In easy AI");
        //this.player = player;//gameControl.playerControl.activePlayer;

        //while actions > 0 && PlayerHasSoldier = false
        while (player.numberOfMoves > 0)
        {
            if (PlayerHasSoldier(player))
            {
                //if (target == null)
                    //find nearest enemy unit
                    List<TileControl> path = pathFinding.FindPathToNearestEnemy(leiginToUse.tileControl, player);
                    int pathLength = path.Count;
                    if (path != null && pathLength > 0)
                    {
                        //assign the target at the end of the path
                        Debug.Log($"got path, setting target");
                        target = path[pathLength - 1].diceOnTile;

                        if (pathLength >= player.numberOfMoves)
                        {
                            Debug.Log($"Enemy if further than number of actions {pathLength} > {player.numberOfMoves}");
                            //move the first x number of moves
                            List<TileControl> pathProgress = path.GetRange(0, player.numberOfMoves);

                            await gameControl.boardControl.HopTo(pathProgress, leiginToUse);
                            if (target.isBase)
                            {
                                gameControl.boardControl.CalculateAttackEnemyBase(leiginToUse, target, pathProgress);
                            } else {
                                gameControl.boardControl.CalculateAttackEnemy(leiginToUse, target, pathProgress);
                            }
                            gameControl.playerControl.TakeMovesFromPlayer(player, player.numberOfMoves);
                        }
                        else
                        {
                            Debug.Log($"Can attack target {target.diceColour} {target.value}");
                            await gameControl.boardControl.HopTo(path, leiginToUse);
                            if (target.isBase)
                            {
                                gameControl.boardControl.CalculateAttackEnemyBase(leiginToUse, target, path);
                            }
                            else
                            {
                                gameControl.boardControl.CalculateAttackEnemy(leiginToUse, target, path);
                            }
                            gameControl.playerControl.TakeMovesFromPlayer(player, pathLength);
                        }
                    } else
                    {
                        Debug.Log($"There is apparently only good guys around {path}");
                    }


                //move towards target
                //todo refine pathfinding to determine how many moves to a position Or move X tile toward a piece.
            } else
            {
                //if player has worker
                //add too worker, 
                //else create dice
                // - pick random side to create dice

                leiginToUse = gameControl.playerControl.GetFirstPlayerWorker(player);
                if (leiginToUse == null)
                {
                    Debug.Log($"No workers found for player: {player.name}");

                    Dice_Control playerOutpost = gameControl.playerControl.GetFirstPlayerOutpost(player);
                    if (playerOutpost != null)
                    {
                        CreateWorkerNextToOupost(playerOutpost, player);
                    } else
                    {
                        Debug.Log($"{player.name} has no Outpost, or workers or soldiers, should be out of the game");
                    }
                } else {
                    Debug.Log($"At least 1 Worker found for player {player.name}");

                    if (FindWorkerBesideOutpost(player))
                    {
                        Debug.Log($"Worker found, adding 1 to it -> {player.name}");
                        Dice_Control playerOutpost = gameControl.playerControl.GetFirstPlayerOutpost(player);//Note for multible outpost, will need to sync with FindWorkerBesideOutpost
                        AddPlussOneToWorkerNextToOutPost(playerOutpost, leiginToUse, player);
                    } else
                    {
                        Debug.Log($"Worker is not next to a outpost");
                        Dice_Control playerOutpost = gameControl.playerControl.GetFirstPlayerOutpost(player);//no worker next to outpost, so just get
                        CreateWorkerNextToOupost(playerOutpost, player);
                    }
                }
            }
        }//end while
        //reset for next ai turn
        target = null;
        leiginToUse = null;
    }

    private void AddPlussOneToWorkerNextToOutPost(Dice_Control outpost, Dice_Control workerToAddToo, Player player) 
    {
        Debug.Log($"Adding + to worker beside {outpost.tileControl} for {player.name}");
        gameControl.boardControl.BaseReenforceAdjacent(workerToAddToo);
        //gameControl.boardControl.BaseReenforceAdjacent(PickAjacentTileToCreateWorker(outpost.tileControl).diceOnTile);
        //gameControl.playerControl.TakeMovesFromPlayer(player);
    }

    private void CreateWorkerNextToOupost(Dice_Control outpost, Player player)
    {
        Debug.Log($"Adding + to worker beside {outpost.tileControl} for {player.name}");

        Debug.Log($"creating worker next to outpost {outpost.tileControl.tileIndex}");
        gameControl.boardControl.CreateDiceAt(PickAjacentTileToCreateWorker(outpost.tileControl), outpost.player);
        gameControl.playerControl.TakeMovesFromPlayer(player);
    }

    internal TileControl PickAjacentTileToCreateWorker(TileControl tileControl)
    {
        //take the tile, and get adjacent.
        // filter out the ones that have
        //if null ahhhhh error
        List<TileControl> candidateTiles = pathFinding.GetAjacentTiles(tileControl, GetAdjacentTilesType.AllEmpty);
        if (candidateTiles.Count == 0)
        {
            Debug.Log($"Ahh, i'm surounded by enemyies, what do i do");
            return pathFinding.GetAjacentTiles(tileControl, GetAdjacentTilesType.All)[0];
        }
        else if (candidateTiles.Count == 1)
        {
            return candidateTiles[0];
        }
        else
        {
            TileControl closesetToEnemy = null;
            List<TileControl> closentBuffer = new List<TileControl>();

            int shortest = 999;
            Player player = tileControl.diceOnTile.player;
            foreach (TileControl tile in candidateTiles)
            {
                closentBuffer = pathFinding.FindPathToNearestEnemy(tile, player);
                if (closentBuffer.Count < shortest)
                {
                    shortest = closentBuffer.Count;
                    closesetToEnemy = closentBuffer[0];
                }
            }
            target = closentBuffer[closentBuffer.Count -1].diceOnTile;
            return closesetToEnemy;
        }
    }

    private bool FindWorkerBesideOutpost(Player player)
    {
        
        List<Dice_Control> workers = gameControl.playerControl.GetAllPlayerWorkers(player);
        if(workers.Count == 1)
        {
            Debug.Log($"1 one worker for player: {player.name}");            

            if (pathFinding.GetAjacentTiles(workers[0].tileControl, GetAdjacentTilesType.AllFriendlyOutposts, player) != null)
            {
                Debug.Log($"worker is next to outpost");
                return true;
            } else
            {
                Debug.Log($"worker not next to outpost");
                return false;
            }
        } else
        {
            Debug.Log($"Multible workers for player: {player.name}");
            foreach( Dice_Control worker in workers)
            {
                if(pathFinding.GetAjacentTiles(worker.tileControl, GetAdjacentTilesType.AllFriendlyOutposts, player) != null)
                {
                    Debug.Log($" - worker {worker.value} {worker.tileControl.tileIndex} is next to outpost");
                    leiginToUse = worker;//set a worker next to an outpost to the active unit

                    //for multible outposts get the tile with the outpost on it// easy ai won't make new outpost, so todo
                    return true;
                }
                else
                {
                    Debug.Log($" - worker {worker.tileControl.tileIndex} not next to outpost");
                }
            }
        }
        return false; //there was no worker next to a base
    }

    private bool PlayerHasSoldier(Player player)
    {
        leiginToUse = gameControl.playerControl.GetFirstPlayerSoldier(player);
        if (leiginToUse != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
