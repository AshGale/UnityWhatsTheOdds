using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public enum AiDifficulty
{
    Easy, Medium, Hard
}

public enum AiState
{
    Growth, Attack, Defend,  Outpost
}

public class AiControl : MonoBehaviour
{

    public GameControl gameControl;
    public PathFinding pathFinding;

    public Dice_Control target;
    public Dice_Control leiginToUse;

    public AiDifficulty difficulty = AiDifficulty.Easy;


    public async void DoTurn(Player player)
    {
        switch (player.ai.difficulty)
        {
            case AiDifficulty.Easy: await EasyAi(player); break;
            case AiDifficulty.Medium: await MediumAi(player); break;
            case AiDifficulty.Hard: await EasyAi(player); break;
        }
    }

    private async Task MediumAi(Player player)
    {

    }

    private async Task EasyAi(Player player)
    {
        Debug.Log($"In easy AI with {player.numberOfMoves}");
        //this.player = player;//gameControl.playerControl.activePlayer;

        //while actions > 0 && PlayerHasSoldier = false
        while (player.numberOfMoves > 0)
        {
            if (PlayerHasSoldier(player))
            {
                //find nearest enemy unit
                List<TileControl> path = pathFinding.FindPathToNearestEnemy(leiginToUse.tileControl, player);
                //int pathLength = path.Count;

                //assign the target at the end of the path
                Debug.Log($"got path, setting target");
                target = path[path.Count - 1].diceOnTile;

                //first element, is own tile. 
                path = path.GetRange(1, path.Count - 1);

                if (path.Count > player.numberOfMoves)
                {
                    Debug.Log($"{leiginToUse.tileControl.tileIndex} too far away from {target.tileControl.tileIndex} {path.Count} > {player.numberOfMoves}");
                    //move the first x number of moves
                    List<TileControl> pathProgress = path.GetRange(0, player.numberOfMoves);

                    Debug.Log($"Moving {pathProgress.Count} our of {path.Count}");

                    await gameControl.boardControl.HopTo(pathProgress, leiginToUse, false);
                    //await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
                    gameControl.playerControl.TakenMove(player.numberOfMoves);
                }
                else
                {
                    Debug.Log($"{leiginToUse.tileControl.tileIndex} can attack {path.Count} away, target {target.tileControl.tileIndex} {target.currentValue}");
                    //await gameControl.boardControl.HopTo(path, leiginToUse);
                    //await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
                    //await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.HOP_DELAY_TIME));
                    if (target.isBase)
                    {
                        Debug.Log($"{leiginToUse.tileControl.tileIndex} {leiginToUse.currentValue} attacking Outpost {target.tileControl.tileIndex} {target.currentValue}");
                        gameControl.boardControl.CalculateAttackEnemyBase(leiginToUse, target, path);
                    }
                    else
                    {
                        Debug.Log($"{leiginToUse.tileControl.tileIndex} {leiginToUse.currentValue} attacking {target.tileControl.tileIndex} {target.currentValue}");
                        gameControl.boardControl.CalculateAttackEnemy(leiginToUse, target, path);
                    }
                    //gameControl.playerControl.TakeMovesFromPlayer(player, pathLength);
                }
                //for easy ai, just look back to start
                target = null;
                leiginToUse = null;
            }
            else
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
                        CreateWorkerNextToOupost(playerOutpost);
                    }
                    else
                    {
                        Debug.Log($"{player.playerName} has no Outpost, or workers or soldiers, should be out of the game");
                    }
                }
                else
                {
                    Debug.Log($"At least 1 Worker found for player {player.playerName}");
                    Dice_Control playerOutpost = gameControl.playerControl.GetFirstPlayerOutpost(player);//Note for multible outpost, will need to sync with FindWorkerBesideOutpost

                    if (FindWorkerBesideOutpost(playerOutpost))
                    {
                        Debug.Log($"Worker found, adding 1 to it -> {player.playerName}");
                        AddPlussOneToWorkerNextToOutPost(playerOutpost, leiginToUse, player);
                    }
                    else
                    {
                        Debug.Log($"Worker is not next to a outpost");
                        CreateWorkerNextToOupost(playerOutpost);
                    }
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));//>>>>) actions taken
        }//end while
        //reset for next ai turn
        target = null;
        leiginToUse = null;
    }

    private void AddPlussOneToWorkerNextToOutPost(Dice_Control outpost, Dice_Control workerToAddToo, Player player)
    {
        Debug.Log($"Adding + 1 to worker beside {outpost.tileControl} for {player.playerName}");
        gameControl.boardControl.BaseReenforceAdjacent(workerToAddToo);
    }

    private void CreateWorkerNextToOupost(Dice_Control outpost)
    {
        
        TileControl tileToCreateWorker = PickAjacentTileToCreateWorker(outpost.tileControl);
        gameControl.boardControl.CreateDiceAt(tileToCreateWorker, outpost.player);
        Debug.Log($"creating worker next to outpost {outpost.tileControl.tileIndex} at {tileToCreateWorker.tileIndex}");
        gameControl.playerControl.TakenMove();
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
            target = closentBuffer[closentBuffer.Count - 1].diceOnTile;
            return closesetToEnemy;
        }
    }

    private bool FindWorkerBesideOutpost(Dice_Control playerOutpost)
    {
        //revise to look next to outposts for a worker.
        List<Dice_Control> workers = gameControl.playerControl.GetAllPlayerWorkers(playerOutpost.player);
        if (workers.Count == 1)
        {
            Debug.Log($"1 worker for player: {playerOutpost.player.playerName} at {workers[0].tileControl.tileIndex}");
            List<TileControl> foundOutpost = pathFinding.GetAjacentTiles(workers[0].tileControl, GetAdjacentTilesType.AllFriendlyOutposts, playerOutpost.player);
            if (foundOutpost.Count > 0)
            {
                Debug.Log($"worker {workers[0].tileControl.tileIndex} is next to outpost at {foundOutpost[0].tileIndex}");
                leiginToUse = workers[0];//set a worker next to an outpost to the active unit
                return true;
            }
            else
            {
                Debug.Log($"worker not next to outpost");
                return false;
            }
        }
        else
        {
            Debug.Log($"Multible workers for player: {playerOutpost.player.playerName}");
            foreach (Dice_Control worker in workers)
            {
                if (pathFinding.GetAjacentTiles(worker.tileControl, GetAdjacentTilesType.AllFriendlyOutposts, playerOutpost.player).Count > 0)
                {
                    Debug.Log($" - worker {worker.currentValue} {worker.tileControl.tileIndex} is next to outpost");
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
            Debug.Log($"Soldier found at {leiginToUse.tileControl.tileIndex}");
            return true;
        }
        else
        {
            Debug.Log($"No soldier found for {player.playerName}");
            return false;
        }
    }

}
