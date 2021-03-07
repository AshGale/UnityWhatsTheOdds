using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public enum AiDifficulty
{
    Player, Easy, Medium, Hard
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
    private int workerValueThreshold = 0;
    private int soldierValueThreshold = 0;
    private int incomeValueThreshold = 0;

    public async void DoTurn(Player player)
    {
        switch (player.ai.difficulty)
        {
            case AiDifficulty.Player: player.is_ai_player = false; break;
            case AiDifficulty.Easy: await AiLogic(player); break;
            case AiDifficulty.Medium: await AiLogic(player); break;
            case AiDifficulty.Hard: await AiLogic(player); break;
        }
    }

    private async Task AiLogic(Player player)
    {
        await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
        Debug.Log($"In {difficulty} AI {player.playerName} income: {player.income} <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
        while (player.numberOfMoves > 0)
        {
            //Rand chance for this ?
            if (OutpostUnderAttack(player))
            {
                await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
                continue;// skip to next iteration
            }

            //dependant on income < threshold (recalculated when action is taken)
            if (player.income < incomeValueThreshold)
            {
                //will make new or add to workers if income less than threshold
                Debug.Log($"Player {player.playerName} Income Below {incomeValueThreshold}");
                FindPrimaryOutPost(player);

                if (primaryOutpost == null)
                {
                    await NoOutpostLogic(player);
                }
                else
                {
                    if (await ImproveIncome(player) == false) 
                        DetermineLastMove(player);                    
                }
            }
            else
            {
                // start to attack
                List<Dice_Control> playerSoldiers = gameControl.playerControl.GetAllPlayerSoldiers(player);
                Debug.Log($"Player {player.playerName} Income Avove {incomeValueThreshold} -> attack with {playerSoldiers.Count} soldiers");

                if (playerSoldiers.Count == 0)
                {
                    if (player.income > (incomeValueThreshold + workerValueThreshold))
                    {
                        if (CreateSoldierFromWorkerNextToOupost(player) == false)
                            await ImproveIncome(player);
                    } else
                    {   
                        await ImproveIncome(player);
                        //DetermineLastMove(player);
                    }
                }
                else if (playerSoldiers.Count == 1)
                {
                    leiginToUse = playerSoldiers[0];
                    Debug.Log($"Soldier {leiginToUse.GetDiceValue()} found at {leiginToUse.tileControl.tileIndex}");
                    await AttackNearestEnemy(leiginToUse, player);
                }
                else
                {
                    Debug.Log($"Multible Soldiers found at {playerSoldiers.Count}");
                    //loop through solders to get closes to enemy
                    //for now just pick last 
                    int last = playerSoldiers.Count;
                    await AttackNearestEnemy(playerSoldiers[last - 1], player);
                }   
            }
            await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
        }
        await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));        
    }

    private bool CreateSoldierFromWorkerNextToOupost(Player player)
    {
        Debug.Log($"No soldier found for {player.playerName}");
        List<TileControl> playersWorksNextToOutpost = pathFinding.GetAjacentTiles(primaryOutpost.tileControl, GetAdjacentTilesType.AllFriendlyWorkersDice, player);

        leiginToUse = null;
        int currentDistanceToEnemy = 9999999;
        List<TileControl> pathToEnemy = new List<TileControl>();
        foreach (TileControl worker in playersWorksNextToOutpost)
        {
            if (worker.diceOnTile.GetDiceValue() <= soldierValueThreshold)
            {
                //logic to ensure the worker converted is the closest to the enemy
                if (leiginToUse == null)
                {
                    //just add
                    //soldierTile.diceOnTile.GetDiceValue() != 6
                    leiginToUse = worker.diceOnTile;
                    pathToEnemy = pathFinding.FindPathToNearestEnemy(leiginToUse.tileControl, player);
                    currentDistanceToEnemy = pathToEnemy.Count;
                    //break;//stop looking
                }
                else
                {
                     pathToEnemy = pathFinding.FindPathToNearestEnemy(worker.diceOnTile.tileControl, player);
                    if (pathToEnemy.Count < currentDistanceToEnemy)
                    {
                        leiginToUse = worker.diceOnTile;
                        currentDistanceToEnemy = pathToEnemy.Count;
                    }
                }                
            }
        }

        if (leiginToUse == null)
        {
            //will have to check for empty spaces first, else move a dice ?
            Debug.Log($"No workers next to outpost {primaryOutpost.tileControl.tileIndex}, below threshold {soldierValueThreshold}");
            //CreateWorkerNextToOupost(primaryOutpost);
            return false;
        }
        else
        {
            Debug.Log($"Adding +1 to make Soldier {leiginToUse.tileControl.tileIndex} next to {primaryOutpost.tileControl.tileIndex}");
            gameControl.boardControl.BaseReenforceAdjacent(leiginToUse);
            return true;
        }
    }

    private async Task<bool> ImproveIncome(Player player)
    {
        if (await CanUpgradeWorker(player)) return true;
        if (CreateWorkerNextToOupost(primaryOutpost)) return true;
        await MoveWorkerOneSquare(player);
        return true;
    }

    private async Task<bool> CanUpgradeWorker (Player player)
    {
        Debug.Log($"Trying to Add 2 too value 1 workers near outpost at {primaryOutpost.tileControl.tileIndex}");
        if (player.numberOfMoves < 2) return false;        

        List<TileControl> workersNextToPrimaryOutpost = 
            pathFinding.GetAjacentTiles(primaryOutpost.tileControl, GetAdjacentTilesType.AllFriendlyWorkersDice, player);

        leiginToUse = null;
        foreach (TileControl workerTile in workersNextToPrimaryOutpost)
        {
            if (workerTile.diceOnTile.GetDiceValue() < workerValueThreshold)
            {
                leiginToUse = workerTile.diceOnTile;
                break;
            }
        }
        if (leiginToUse != null)
        {
            await UpgradeWorker(leiginToUse);
            return true;
        }
        return false;
    }

    private async Task UpgradeWorker(Dice_Control worker)
    {
        Debug.Log($"Adding 2 too worker at {worker.tileControl.tileIndex}");
        gameControl.boardControl.BaseReenforceAdjacent(worker);
        await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.AI_ACTION_SPEED));
        gameControl.boardControl.BaseReenforceAdjacent(worker);
    }
    private async Task NoOutpostLogic(Player player)
    {
        Dice_Control dice_Control;
        int playerValue = 0;
        foreach (GameObject dice in player.diceOwned)
        {
            dice_Control = dice.GetComponent<Dice_Control>();
            playerValue += dice_Control.GetDiceValue();
            if (playerValue >= 12) break;
        }

        if (playerValue >= 12)
        {
            Debug.Log($"Player {player.playerName} value greater than 12, can make another outpost");
            TryToCreateNewOutpost(player);
        }
        else
        {
            Debug.Log($"Player {player.playerName} < 12 attack enemy");

            if (await GetSolderAndAttackEnemy(player) == false)
            {
                //create soldier from workers
                //get worker nearest enemy, get workers nearest to that one, then combine to create a soldier
                Debug.Log($"Player {player.playerName} only has workers >>>>> not implemented to find nearby workers, to make a solder, to attack");
            }
        }
    }

    private async void DetermineLastMove(Player player)
    {
        Debug.Log($"Determining LastMove for {leiginToUse.tileControl} ");
        if (CreateWorkerNextToOupost(primaryOutpost)) return;
        if (ReenforceSolder(player)) return;
        if (await GetSolderAndAttackEnemy(player)) return;
        await MoveWorkerOneSquare(player);
    }

    private async Task MoveWorkerOneSquare(Player player)
    {
        //get all the workers beside the outpost
        List<TileControl> playersWorksNextToOutpost = pathFinding.GetAjacentTiles(primaryOutpost.tileControl, GetAdjacentTilesType.AllFriendlyWorkersDice, player);

        //nb should never have soldiers as, that was taken care of in a previous step.

        //move the last one to a corner tile of the outpost
        int last = playersWorksNextToOutpost.Count -1;
        leiginToUse = playersWorksNextToOutpost[last].diceOnTile;

        List<TileControl> emptyTilesForLeigin = pathFinding.GetAjacentTiles(leiginToUse.tileControl, GetAdjacentTilesType.AllEmpty);
        Debug.Log($"moving dice {leiginToUse.tileControl} one square at {emptyTilesForLeigin [0].tileIndex}");
        await gameControl.boardControl.HopTo(new List<TileControl> { emptyTilesForLeigin [0]}, leiginToUse, false);
    }

    private void TryToCreateNewOutpost(Player player)
    {
        //find 2 highest value pices and move then onto same tile
        //try to create workers on combinations. // will probably have to do chaining of actions
        //then move closes not highest dice to save area untill you get 2 6's. 
        //combine dice to create outpost 

        throw new NotImplementedException();
    }

    private bool ReenforceSolder(Player player)
    {
        List<TileControl> soldiersNextToPrimaryOutpost = pathFinding.GetAjacentTiles(primaryOutpost.tileControl, GetAdjacentTilesType.AllFriendlySoldierDice, player);

        leiginToUse = null;
        foreach (TileControl soldierTile in soldiersNextToPrimaryOutpost)
        {
            if (soldierTile.diceOnTile.GetDiceValue() < soldierValueThreshold)
            {
                if (soldierTile.diceOnTile.GetDiceValue() != 6)
                {
                    leiginToUse = soldierTile.diceOnTile;
                    break;//stop looking
                }                
            }
        }

        if (leiginToUse == null)
        {
            return false;
        }
        else
        {
            Debug.Log($"Adding +1 to Solder {leiginToUse.tileControl.tileIndex} next to {primaryOutpost.tileControl.tileIndex}");
            gameControl.boardControl.BaseReenforceAdjacent(leiginToUse);
            return true;
        }
    }

    

    private async Task<bool> GetSolderAndAttackEnemy(Player player)
    {
        List<Dice_Control> playerSoldiers = gameControl.playerControl.GetAllPlayerSoldiers(player);

        if (playerSoldiers.Count == 0)
        {
            return false;
        }
        else if (playerSoldiers.Count == 1)
        {
            leiginToUse = playerSoldiers[0];
            Debug.Log($"Soldier {leiginToUse.GetDiceValue()} found at {leiginToUse.tileControl.tileIndex}");
            await AttackNearestEnemy(leiginToUse, player);
        }
        else
        {
            Debug.Log($"Multible Soldiers found at {playerSoldiers.Count}");
            //loop through solders to get closes to enemy
            //for now just pick last 
            int last = playerSoldiers.Count;
            await AttackNearestEnemy(playerSoldiers[last - 1], player);
        }
        return true;
    }

    private async Task<bool> AttackNearestEnemy(Dice_Control soldier, Player player)
    {
        //find nearest enemy unit
        List<TileControl> path = pathFinding.FindPathToNearestEnemy(soldier.tileControl, player);
        if (path.Count == 0) return false;

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

            gameControl.playerControl.TakenMove(player.numberOfMoves);
            await gameControl.boardControl.HopTo(pathProgress, soldier, false);
            
        }
        else
        {
            Debug.Log($"{soldier.tileControl.tileIndex} can attack {path.Count} away, target {target.tileControl.tileIndex} {target.currentValue}");

            if (target.isBase)
            {
                Debug.Log($"{soldier.tileControl.tileIndex} {soldier.currentValue} attacking Outpost {target.tileControl.tileIndex} {target.currentValue}");
                gameControl.boardControl.CalculateAttackEnemyBase(soldier, target, path);
            }
            else
            {
                Debug.Log($"{soldier.tileControl.tileIndex} {soldier.currentValue} attacking {target.tileControl.tileIndex} {target.currentValue}");
                await gameControl.boardControl.CalculateAttackEnemy(soldier, target, path);
            }
        }
        return true;
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

   
    private bool CreateWorkerNextToOupost(Dice_Control outpost)
    {
        List<TileControl> candidateTiles = pathFinding.GetAjacentTiles(outpost.tileControl, GetAdjacentTilesType.AllEmpty);
        if (candidateTiles.Count == 0) return false;

        TileControl tileToCreateWorker = PickAjacentTileNearestToEnemy(candidateTiles, outpost.player);
        gameControl.boardControl.CreateDiceAt(tileToCreateWorker, outpost.player);
        Debug.Log($"creating worker next to outpost {outpost.tileControl.tileIndex} at {tileToCreateWorker.tileIndex}");
        gameControl.playerControl.TakenMove();
        return true;
    }

    private TileControl PickAjacentTileNearestToEnemy(List<TileControl> candidateTiles, Player player)
    {
        
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

    //------------------------------------------------------------------------------------------------------------------------------

    internal void SetDifficulty(AiDifficulty AIdifficulty)
    {
        this.difficulty = AIdifficulty;
        switch (difficulty)
        {
            case AiDifficulty.Easy:
                {
                    workerValueThreshold = GlobalVariables.data.EASY_AI_WORKER_VALUE_THRESHOLD;
                    soldierValueThreshold = GlobalVariables.data.EASY_AI_SOLDIER_VALUE_THRESHOLD;
                    incomeValueThreshold = GlobalVariables.data.EASY_AI_INCOME_THRESHOLD;
                    break;
                }
            case AiDifficulty.Medium:
                {
                    workerValueThreshold = GlobalVariables.data.MEDIUM_AI_WORKER_VALUE_THRESHOLD;
                    soldierValueThreshold = GlobalVariables.data.MEDIUM_AI_SOLDIER_VALUE_THRESHOLD;
                    incomeValueThreshold = GlobalVariables.data.MEDIUM_AI_INCOME_THRESHOLD;
                    break;
                }
            case AiDifficulty.Hard:
                {
                    workerValueThreshold = GlobalVariables.data.HARD_AI_WORKER_VALUE_THRESHOLD;
                    soldierValueThreshold = GlobalVariables.data.HARD_AI_SOLDIER_VALUE_THRESHOLD;
                    incomeValueThreshold = GlobalVariables.data.HARD_AI_INCOME_THRESHOLD;
                    break;
                }
        }
    }
}
