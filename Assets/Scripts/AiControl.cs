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
    public Dice_Control primaryOutpost;

    public AiDifficulty difficulty;


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
        await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
        Debug.Log($"In Medium AI {player.playerName} income: {player.income}");
        while (player.numberOfMoves > 0)
        {
            if (OutpostUnderAttack(player))
            {
                await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
                continue;// skip to next iteration
            }

            //dependant on income < threshold (recalculated when action is taken)
            if (player.income < GlobalVariables.data.MEDIUM_AI_INCOME_THRESHOLD)
            {
                //will make new or add to workers if income less than threshold

                FindPrimaryOutPost(player);

                if (primaryOutpost == null)
                {
                    //need to implement of ai will infinate loop and game will freze<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                    //kinda in end game for player. 
                    //if all pices value >= 12 move to make a base withough leaving self with 0 income next round
                    //else
                    //if has soldier, attack nearest
                    //else move close to nearest enemy
                    //if number of dice > 1 combine dice, and attack
                }
                else
                {
                    //
                    if(player.numberOfMoves >= 2)
                    {
                        //todo, loop through all outposts and add together
                        Debug.Log($"Trying to Add 2 too value 1 workers near outpost at {primaryOutpost.tileControl.tileIndex}");
                        List<TileControl> workersNextToPrimaryOutpost = pathFinding.GetAjacentTiles(primaryOutpost.tileControl, GetAdjacentTilesType.AllFriendlyWorkersDice, player);

                        //is there workers with value lower than threshold                   
                        leiginToUse = null;
                        foreach (TileControl workerTile in workersNextToPrimaryOutpost)
                        {
                            if (workerTile.diceOnTile.GetDiceValue() < GlobalVariables.data.MEDIUM_AI_WORKER_VALUE_THRESHOLD)
                            {
                                leiginToUse = workerTile.diceOnTile;
                                break;
                            }
                        }

                        if (leiginToUse != null)
                        {
                            Debug.Log($"Adding 2 too worker at {leiginToUse.tileControl.tileIndex}");
                            gameControl.boardControl.BaseReenforceAdjacent(leiginToUse);
                            await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
                            gameControl.boardControl.BaseReenforceAdjacent(leiginToUse);
                        }
                        else
                        {
                            Debug.Log($"No Workers below worker value threshold {GlobalVariables.data.MEDIUM_AI_WORKER_VALUE_THRESHOLD}");
                            CreateWorkerNextToOupost(primaryOutpost);
                        }                        
                    }
                    else
                    {           
                        //for when the player only has one move left
                        Debug.Log($"Trying to inprove soldiers near base first at {primaryOutpost.tileControl.tileIndex}");
                        List<TileControl> soldiersNextToPrimaryOutpost = pathFinding.GetAjacentTiles(primaryOutpost.tileControl, GetAdjacentTilesType.AllFriendlySoldierDice, player);

                        if (soldiersNextToPrimaryOutpost.Count == 0)
                        {
                            Debug.Log($"No Soldiers next to outpost {primaryOutpost.tileControl.tileIndex}");
                            CreateWorkerNextToOupost(primaryOutpost);
                        }
                        else if (soldiersNextToPrimaryOutpost.Count == 1)
                        {
                            // filter out solders at value 6 TODO
                            Dice_Control soldier = soldiersNextToPrimaryOutpost[0].diceOnTile;
                            if (soldier.GetDiceValue() == 6)
                            {
                                CreateWorkerNextToOupost(primaryOutpost);
                            }
                            else
                            {
                                Debug.Log($"Adding +1 to Solder {soldiersNextToPrimaryOutpost[0].tileIndex} next to {primaryOutpost.tileControl.tileIndex}");
                                gameControl.boardControl.BaseReenforceAdjacent(soldier);
                            }
                        }
                        else
                        {
                            leiginToUse = null;
                            foreach (TileControl soldierTile in soldiersNextToPrimaryOutpost)
                            {
                                if (soldierTile.diceOnTile.GetDiceValue() != 6)
                                {
                                    leiginToUse = soldierTile.diceOnTile;
                                    break;//stop looking
                                }
                            }
                            if (leiginToUse == null)
                            {
                                CreateWorkerNextToOupost(primaryOutpost);
                            }
                            else
                            {
                                Debug.Log($"Adding +1 to Solder {leiginToUse.tileControl.diceOnTile} next to {primaryOutpost.tileControl.tileIndex}");
                                gameControl.boardControl.BaseReenforceAdjacent(leiginToUse);
                            }
                        }
                    }
                }
            }
            else
            {
                // start to attack

                if (gameControl.playerControl.GetFirstPlayerSoldier(player) != null)//could be check on play object?
                {
                    List<Dice_Control> playerSoldiers = gameControl.playerControl.GetAllPlayerSoldiers(player);

                    if(playerSoldiers.Count == 1)
                    {
                        Debug.Log($"Soldier found at {playerSoldiers[0].tileControl.tileIndex}");
                        //attack with solder to nearest enemy
                        AttackNearestEnemy(playerSoldiers[0], player);
                    } else
                    {
                        Debug.Log($"Multible Soldiers found at {playerSoldiers.Count}");
                        //loop through solders to get closes to enemy
                        //for now just pick last 
                        int last = playerSoldiers.Count;
                        AttackNearestEnemy(playerSoldiers[last -1], player);
                    }
                } else
                {
                    Debug.Log($"No soldier found for {player.playerName}");

                    //need to create soldier next to outpost 
                    //NB if no outpost exist, and got here, then player only has workers, with income > threshold. attempt to make new outpost, or go to endgame logic
                    List<TileControl> playersWorksNextToOutpost = pathFinding.GetAjacentTiles(primaryOutpost.tileControl, GetAdjacentTilesType.AllFriendlyWorkersDice, player);

                    if(playersWorksNextToOutpost.Count == 0)
                    {
                        Debug.Log($"No workers next to outpost found at {primaryOutpost.tileControl}");
                        //ie, no solders next to outpost, no workers next to outpost. 
                        //todo make logic for the closest pieces to combine to maek a base if possable, or closetst to attack
                    } else if (playersWorksNextToOutpost.Count == 1)
                    {
                        gameControl.boardControl.BaseReenforceAdjacent(playersWorksNextToOutpost[0].diceOnTile);
                    } else
                    {
                        TileControl workerNearestToEnemy = PickAjacentTileNearestToEnemy(playersWorksNextToOutpost, player);
                        gameControl.boardControl.BaseReenforceAdjacent(workerNearestToEnemy.diceOnTile);
                    }

                    //for hard, find two workers that are not next ot outpost and combine
                }

            }
            await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
        }
        await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));

        
    }

    private async void AttackNearestEnemy(Dice_Control soldier, Player player)
    {
        //find nearest enemy unit
        List<TileControl> path = pathFinding.FindPathToNearestEnemy(soldier.tileControl, player);
        //int pathLength = path.Count;

        //assign the target at the end of the path
        Debug.Log($"got path, setting target");
        target = path[path.Count - 1].diceOnTile;

        //first element, is own tile. 
        path = path.GetRange(1, path.Count - 1);

        if (path.Count > player.numberOfMoves)
        {
            Debug.Log($"{soldier.tileControl.tileIndex} too far away from {target.tileControl.tileIndex} {path.Count} > {player.numberOfMoves}");
            //move the first x number of moves
            List<TileControl> pathProgress = path.GetRange(0, player.numberOfMoves);

            Debug.Log($"Moving {pathProgress.Count} our of {path.Count}");

            await gameControl.boardControl.HopTo(pathProgress, soldier, false);
            //await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
            gameControl.playerControl.TakenMove(player.numberOfMoves);
        }
        else
        {
            Debug.Log($"{soldier.tileControl.tileIndex} can attack {path.Count} away, target {target.tileControl.tileIndex} {target.currentValue}");
            //await gameControl.boardControl.HopTo(path, leiginToUse);
            //await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
            //await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.HOP_DELAY_TIME));
            if (target.isBase)
            {
                Debug.Log($"{soldier.tileControl.tileIndex} {soldier.currentValue} attacking Outpost {target.tileControl.tileIndex} {target.currentValue}");
                gameControl.boardControl.CalculateAttackEnemyBase(soldier, target, path);
            }
            else
            {
                Debug.Log($"{soldier.tileControl.tileIndex} {soldier.currentValue} attacking {target.tileControl.tileIndex} {target.currentValue}");
                gameControl.boardControl.CalculateAttackEnemy(soldier, target, path);
            }
            //gameControl.playerControl.TakeMovesFromPlayer(player, pathLength);
        }
    }

    private bool OutpostUnderAttack(Player player)
    {
        // if anyDice next to Outpost
        List<Dice_Control> playerOutposts = gameControl.playerControl.GetAllPlayerOutpost(player);
        List<TileControl> adjacentTilesWithDice;
        if (playerOutposts.Count == 1)
        {
            Debug.Log($"1 Outpost for {player.playerName}");
            primaryOutpost = playerOutposts[0];
            adjacentTilesWithDice = pathFinding.GetAjacentTiles(primaryOutpost.tileControl, GetAdjacentTilesType.AllEnemyDice, player);
            if (adjacentTilesWithDice.Count > 0)
            {
                Debug.Log($"Enemy Dice found next to outpost {primaryOutpost.tileControl.tileIndex}");
                if (adjacentTilesWithDice.Count == 1)
                {
                    gameControl.boardControl.CalculateBasesAttackEnemy(primaryOutpost, adjacentTilesWithDice[0].diceOnTile);
                }
                else
                {
                    //select best one to attack//for now, just select last
                    int last = adjacentTilesWithDice.Count;
                    gameControl.boardControl.CalculateBasesAttackEnemy(primaryOutpost, adjacentTilesWithDice[last - 1].diceOnTile);
                }
                return true;
            }
            else
            {
                Debug.Log($"Oupost Safe at {primaryOutpost.tileControl.tileIndex}");         
            }
        }
        else if (playerOutposts.Count > 1)
        {
            Debug.Log($"Multible Outposts for {player.playerName} TODO");
            //loop through each each outpost to see if enemy beside 
        }
        else
        {
            Debug.Log($"No Outposts for {player.playerName}");
            primaryOutpost = null;
            return false;
        }
        
        return false;
    }

    private void FindPrimaryOutPost(Player player)
    {
        List<Dice_Control> playerOutposts = gameControl.playerControl.GetAllPlayerOutpost(player);

        if (playerOutposts.Count == 1)
        {
            //Debug.Log($"1 Outpost for {player.playerName}");
            primaryOutpost = playerOutposts[0];
        }
        else if (playerOutposts.Count > 1)
        {
            Debug.Log($"Multible Outposts for {player.playerName} TODO");
            //for now just pick last
            int last = playerOutposts.Count;
            primaryOutpost = playerOutposts[last - 1];
        }
        else
        {
            Debug.Log($"No Outposts for {player.playerName}");
            primaryOutpost = null;
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------

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
        List<TileControl> candidateTiles = pathFinding.GetAjacentTiles(outpost.tileControl, GetAdjacentTilesType.AllEmpty);
        TileControl tileToCreateWorker = PickAjacentTileNearestToEnemy(candidateTiles, outpost.player);
        gameControl.boardControl.CreateDiceAt(tileToCreateWorker, outpost.player);
        Debug.Log($"creating worker next to outpost {outpost.tileControl.tileIndex} at {tileToCreateWorker.tileIndex}");
        gameControl.playerControl.TakenMove();
    }

    private TileControl PickAjacentTileNearestToEnemy(List<TileControl> candidateTiles, Player player)
    {
        //take the tile, and get adjacent.
        // filter out the ones that have
        //if null ahhhhh error
        
        if (candidateTiles.Count == 0)
        {
            Debug.Log($"Logical Error. Need to move a piece in this case instead?");
            return null;
        }
        else if (candidateTiles.Count == 1)
        {
            return candidateTiles[0];
        }
        else
        {
            TileControl closesetToEnemy = null;
            List<TileControl> closentBuffer = new List<TileControl>();

            int shortest = 9999999;
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
