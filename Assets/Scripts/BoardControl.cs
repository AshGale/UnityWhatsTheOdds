using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BoardControl : MonoBehaviour
{
    public GameObject Tile;
    public GameObject Dice;
    public Material WhiteMaterial;
    public Material BlackMaterial;
    private int width;
    private int depth;
    private float tileSize;
    private string tileName;
    private bool WhiteTile;

    public GameControl gameControl;
    public TileControl[,] boardTiles;
    readonly List<Vector2Int> adjacent = new List<Vector2Int> { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    internal IEnumerator KickOffSetup()
    {
        //yield return new WaitForSeconds(.5f);//allow for setup to finish
        width = GlobalVariables.data.WIDTH;
        depth = GlobalVariables.data.DEPTH;
        tileSize = GlobalVariables.data.TILE_SIZE;
        tileName = GlobalVariables.data.TILE_NAME;
        WhiteTile = true;
        boardTiles = new TileControl[width, depth];
        GenerateBoard();
        yield return null;
    }

    public TileControl[,] GetBoardTiles()
    {
        return this.boardTiles;
    }

    //TODO move out to pahfinding fully

    internal List<TileControl> Pathfind(TileControl currentTile, TileControl clickedTile, int playerMoves)
    {
        //NOte for a* will need to have loop of:
        // set valid adjacnt, to move count
        // for each tile, set to targetX-tileX + targetZ-tileZ as positive// note this is for no horizontal only
        // track lowest dist to target, and move count, to pic tile, or get first

        ////optimisation for short adjacent tiles, or long straight move
        //bool allValid = true;
        //List<TileControl> path = new List<TileControl>();
        //if (currentTile.tileIndex.x == clickedTile.tileIndex.x)
        //{
        //    Debug.Log($"target is straight ahead or behind");
        //    int x = currentTile.tileIndex.x;
        //    TileControl tileToTest;
        //    //check if all Y tiles vailed towards target, if true, lead too
        //    if (currentTile.tileIndex.y < clickedTile.tileIndex.y)//eg (3,4) --> (3,7): 7-5= 2 [5,6]
        //    {
        //        if ((clickedTile.tileIndex.y - currentTile.tileIndex.y) > playerMoves) { Debug.Log($"too far to target {(clickedTile.tileIndex.y - currentTile.tileIndex.y)} > {playerMoves}"); return null; }
        //        for (int y = currentTile.tileIndex.y + 1; y < clickedTile.tileIndex.y; y++)
        //        {
        //            tileToTest = boardTiles[x, y];
        //            if (tileToTest.validMove)
        //            {
        //                Debug.Log($"Tile valid {tileToTest.tileIndex}");
        //                path.Add(tileToTest);
        //            } else
        //            {
        //                allValid = false;
        //                Debug.Log($"Invalid {tileToTest.tileIndex}");
        //                break; //for
        //            }
        //        }
        //    }
        //    else //eg (3,7) --> (3,4): 7-5= 2 [6,5]
        //    {
        //        for (int y = currentTile.tileIndex.y - 1; y > clickedTile.tileIndex.y; y--)
        //        {
        //            tileToTest = boardTiles[x, y];
        //            if (tileToTest.validMove)
        //            {
        //                //ok and add to path
        //            }
        //            else
        //            {
        //                //stop loop, and ure obstical avoidance avoidance method
        //            }
        //        }
        //    }
        //}
        //else if (currentTile.tileIndex.y == clickedTile.tileIndex.y)
        //{
        //    //check if all X tiles vailed towards target, if true, lead too
        //    Debug.Log($"target is directly right or left");

        //}
        //if (allValid)
        //{
        //    path.Add(clickedTile);
        //    return path;
        //} 

        clickedTile.SetValidPathfing();
        return FindPathToTile(currentTile, clickedTile, playerMoves);
    }

    public List<TileControl> FindPathToTile(TileControl sorceTile, TileControl targetTile, int playerMoves)//
    {
        BroadcastMessage("ResetPathValue");
        List<TileControl> adjacentTiles = new List<TileControl>();
        List<TileControl> adjacentTilesBuffer = new List<TileControl>();

        //Debug.Log($"Looking for path from {sorceTile.tileIndex} to {targetTile.tileIndex}");

        bool searchForPath = true;
        bool pathFound = false;
        int pathCost = 0;
        adjacentTiles.Add(sorceTile);
        while (searchForPath)
        {
            foreach (TileControl tile in adjacentTiles)
            {
                if (tile == targetTile)
                {                    
                    pathFound = true;
                    searchForPath = false;
                    tile.pathValue = pathCost;
                    if (targetTile.diceOnTile != null) targetTile.SetCanNotPassThrough();
                    //Debug.Log($"Reached Target tile: {tile.tileIndex} cost {tile.pathValue}");
                    break;//for
                }
                else
                {
                    tile.pathValue = pathCost;
                    foreach (TileControl validTile in GetValidAjacentTiles(tile))
                    {
                        adjacentTilesBuffer.Add(validTile);//for next loop
                        //Debug.Log($"added {validTile.tileIndex} to tile buffer");
                    }
                }
            }
            if (pathFound == false)
            {
                if (adjacentTilesBuffer.Count == 0)
                {
                    //Debug.Log($"No more tiles to find, Invalid path to target");
                    searchForPath = false;
                } else
                {
                    pathCost++;
                    if(pathCost > playerMoves)
                    {
                        searchForPath = false; 
                        //Debug.Log($"destination takes too many moves {pathCost} -> {playerMoves}");                        
                    }
                    //A* this is where you would check for closest to target, targetX-tileX + targetZ-tileZ as positive
                    adjacentTiles = adjacentTilesBuffer.ToList();//due to C#ness need to specify new list
                    adjacentTilesBuffer.Clear();
                }
            }
        }
        if (pathFound)
        {
            //Debug.Log($"Path to target exists, retriving. . .");            
            return GetPath(targetTile);
        }
        else
        {
            //Debug.Log($"No path to target found");
            InvalidMove(targetTile);
            return null;
        }        
    }

    private List<TileControl> GetValidAjacentTiles(TileControl tile)//
    {
        List<TileControl> validAdjacentTiles = new List<TileControl>();

        Vector2Int nextIndex;
        TileControl nextTile;
        foreach (Vector2Int direction in adjacent)
        {
            nextIndex = tile.tileIndex + direction;
            //Debug.Log($"{tile.tileIndex}+{direction}={nextIndex}");
            if (nextIndex.x < 0 || nextIndex.y < 0 || nextIndex.x >= width || nextIndex.y >= depth)
            {
                //Debug.Log($"Skipped tile: {nextIndex} out of bounds");
            } else
            {                
                nextTile = boardTiles[nextIndex.x, nextIndex.y];
                if (nextTile.validPathfindingTile == false || nextTile.pathValue >= 0) //todo, will have to update and broadcase valid moves, ie dice on them, is invalid
                {
                    //Debug.Log($"Skipped {nextTile.validMove} tile {nextTile.pathValue}: {nextTile.tileIndex}");
                }
                else
                {
                    validAdjacentTiles.Add(nextTile);
                }
            }
        }   
        return validAdjacentTiles;
    }

    private List<TileControl> GetPathTilesAjacent(TileControl tile)
    {
        List<TileControl> pathAdjacentTiles = new List<TileControl>();

        Vector2Int nextIndex;
        TileControl nextTile;
        foreach (Vector2Int direction in adjacent)
        {
            nextIndex = tile.tileIndex + direction;
            //Debug.Log($"{tile.tileIndex}+{direction}={nextIndex}");
            if (nextIndex.x < 0 || nextIndex.y < 0 || nextIndex.x >= width || nextIndex.y >= depth)
            {
                //Debug.Log($"Skipped path tile: {nextIndex} out of bounds");
            }
            else
            {
                nextTile = boardTiles[nextIndex.x, nextIndex.y];
                if (nextTile.pathValue == -1)
                {
                    //Debug.Log($"Not considered for path: {nextTile.tileIndex}");
                }
                else 
                { 
                    pathAdjacentTiles.Add(nextTile); 
                }                    
            }
        }
        return pathAdjacentTiles;
    }

    private List<TileControl> GetPath(TileControl targetTile)//
    {
        List<TileControl> path = new List<TileControl>() { targetTile };

        List<TileControl> tileBuffer;
        TileControl shortestTile = targetTile;
        bool gettingPath = true;
        while (gettingPath)
        {
            tileBuffer = GetPathTilesAjacent(shortestTile);
            foreach (var tile in tileBuffer)
            {
                if (tile.pathValue == 0)
                {
                    //Debug.Log($"at target {targetTile.tileIndex} = {tile.tileIndex}");
                    gettingPath = false;
                    shortestTile = tile;
                    return path;
                }
                else if (tile.pathValue < shortestTile.pathValue)
                {
                    //Debug.Log($"closer tile found {tile}");
                    shortestTile = tile;
                }                
            }
            path.Insert(0, shortestTile);
            shortestTile = path.First();
        }
        return null;
    }
    //-----------------------------------------end path logic
    //-----------------------------------------start hop logic

    private async Task HopAnimation(Dice_Control diceToMove, TileControl tile)
    {
        //Debug.Log($"Starting to Hop from {diceToMove.tileControl.tileIndex} too {tile.tileIndex}");
        float startTime = Time.time;
        Vector3 origin = diceToMove.transform.position;
        Vector3 destination = tile.transform.position;
        float journeyLength = 1;
        float distCovered;
        float fractionOfJourney = 0;

        origin.y = GlobalVariables.data.BOARD_CLEARANCE;
        diceToMove.transform.position = origin;

        destination.y = GlobalVariables.data.TILE_CLEARANCE;

        while (fractionOfJourney <= 1)
        {
            distCovered = (Time.time - startTime) * GlobalVariables.data.MOVE_SPEED;

            fractionOfJourney = distCovered / journeyLength;
            diceToMove.transform.position = Vector3.Lerp(origin, destination, fractionOfJourney);

            if (fractionOfJourney >= 1)
            {
                //snap to tile logic
                diceToMove.transform.position = new Vector3((float)Mathf.Round(destination.x),
                    diceToMove.transform.position.y,
                    (float)Mathf.Round(destination.z));
            }
            else
            {
                await Task.Yield();
            }
        }
        UpdateBoardMeta(diceToMove.tileControl, tile);
    }

    public async Task HopTo(List<TileControl> path, Dice_Control diceToMove, bool targetTileHasDice = true)
    {
        int pathLength = path.Count;

        if (targetTileHasDice)
        {
            pathLength--;
        }
        if (pathLength > 0)
        {
            //Debug.Log($"enter hopping to tile");
            for (int i = 0; i < pathLength; i++)
            {
                await HopAnimation(diceToMove, path[i]);
                await Task.Delay(TimeSpan.FromSeconds(GlobalVariables.data.HOP_DELAY_TIME));
            }
            //Debug.Log($"exit hopping to tile");
        }
        else
        {
            //Debug.Log($"Dice already next to Target");
            if (targetTileHasDice == false)
            {
                UpdateBoardMeta(diceToMove.tileControl, path[0]);
            }
        }
    }
    //-----------------------------------------end hop logic
    //-----------------------------------------start dice actions

    internal async void MoveToEmptyTile(List<TileControl> path, Dice_Control diceToMove)
    {
        //Debug.Log($"Moving to Empty Tile");
        await HopTo(path, diceToMove, false);
        gameControl.playerControl.TakenMove(path.Count);
    }

    internal void BaseReenforceAdjacent(Dice_Control targetDice)
    {
        if (targetDice.value < 6)
        {
            StartCoroutine(targetDice.ChangeDiceValue(targetDice.value + 1));
            gameControl.playerControl.TakenMove();
        }
        else
        {
            //Debug.Log($"Invlaid Move {targetDice.value} at maximum");
            InvalidMove(targetDice.tileControl);
        }
    }

    internal void CalculateBasesAttackEnemy(Dice_Control OwenDice, Dice_Control targetDice)
    {
        if(targetDice.isBase)
        {
            //Debug.Log($"Invlaid Move bases can't attack bases");
            InvalidMove(targetDice.tileControl);
        }
        else
        {
            if (targetDice.value > 1)
            {
                StartCoroutine(targetDice.ChangeDiceValue(targetDice.value - 1));//defending base attack value

            } else
            {
                targetDice.tileControl.RemoveDiceOnTile();
                DestroySingleDice(targetDice);                
            }
            gameControl.playerControl.TakenMove(1);
            gameControl.AllowInput();
        }
    }

    internal async void CombineFriendlyDice(Dice_Control selectedDice, Dice_Control targetDice, List<TileControl> path)
    {
        if (selectedDice.GetInstanceID() == targetDice.GetInstanceID())
        {
            InvalidMove(targetDice.tileControl);
            return; 
        }

        int sum = selectedDice.value + targetDice.value;
        if (targetDice.isBase)
        {
            if (sum > 6)
            {
                Debug.Log($"Reenforcing outpost from {targetDice.value} to 6");
                await HopTo(path, selectedDice);
                StartCoroutine(targetDice.ChangeDiceValue(6));
                StartCoroutine(selectedDice.ChangeDiceValue(sum - 6));
            }else if (sum == 6)
            {
                Debug.Log($"Absorbing unit, from {targetDice.value} to 6");
                await HopTo(path, selectedDice);
                StartCoroutine(targetDice.ChangeDiceValue(6));
                DestroySingleDice(selectedDice);
                UpdateBoardMeta(selectedDice.tileControl, targetDice.tileControl, false);
            }else
            {
                Debug.Log($"Reenforcing outpost from {targetDice.value} to " + sum);
                await HopTo(path, selectedDice);
                StartCoroutine(targetDice.ChangeDiceValue(sum));
                DestroySingleDice(selectedDice);
                UpdateBoardMeta(selectedDice.tileControl, targetDice.tileControl, false);
            }
            //targetDice.SetSelected();//crashes here
            //gameControl.currentySelected = targetDice;
            gameControl.playerControl.TakenMove(path.Count);
            gameControl.AllowInput();
            return;
        }
        else if (sum >= 12)
        {
            if (targetDice.isBase)
            {
                Debug.Log($"Target already base {targetDice.tileControl.tileIndex}");
                InvalidMove(targetDice.tileControl);
                return;
            }
            gameControl.currentySelected = null;
            await HopTo(path, selectedDice);
            UpdateBoardMeta(selectedDice.tileControl, targetDice.tileControl);
            Debug.Log($"Making new Base at {targetDice.tileControl.tileIndex}");
            CreateBase(selectedDice, targetDice);
        }
        else if (sum > 6)
        {
            if (targetDice.value == 6) { 
                Debug.Log($"Invlaid Move target at maximum");
                InvalidMove(targetDice.tileControl); 
                return; 
            }
            Debug.Log($"Setting target from {targetDice.value} to 6");
            await HopTo(path, selectedDice);
            StartCoroutine(targetDice.ChangeDiceValue(6));
            StartCoroutine(selectedDice.ChangeDiceValue(sum - 6));
        } else if (sum <= 6)
        {
            await HopTo(path, selectedDice);
            Debug.Log($"Combining dice, {sum}");
            StartCoroutine(targetDice.ChangeDiceValue(sum));
            DestroySingleDice(selectedDice);
            UpdateBoardMeta(selectedDice.tileControl, targetDice.tileControl, false);
        }
        selectedDice.SetDeselected();
        targetDice.SetSelected();
        gameControl.currentySelected = targetDice;
        gameControl.playerControl.TakenMove(path.Count);
        gameControl.AllowInput();
    }

    internal async void CalculateAttackEnemy(Dice_Control attackingDice, Dice_Control targetEnemyDice, List<TileControl> path)
    {
        if (attackingDice.value %2 ==0)
        {
            int remainder = targetEnemyDice.value - attackingDice.value; 
            if (remainder < 0)
            {
                Debug.Log($"Attacker won {-remainder} remaining");
                await HopTo(path, attackingDice, true);
                
                DestroySingleDice(targetEnemyDice);
                StartCoroutine(attackingDice.ChangeDiceValue(-remainder));
                MoveToEmptyTile(new List<TileControl> { path.Last() }, attackingDice);
            } else if (remainder == 0)
            {
                Debug.Log($"Tie, both destroyed");
                await HopTo(path, attackingDice, true);

                DestroySingleDice(attackingDice);
                DestroySingleDice(targetEnemyDice);
                
            } else
            {
                Debug.Log($"Defender now at {remainder}");
                await HopTo(path, attackingDice, true);

                StartCoroutine(targetEnemyDice.ChangeDiceValue(remainder));
                DestroySingleDice(attackingDice);
            }
            gameControl.playerControl.TakenMove(path.Count);
            gameControl.AllowInput();
        }
        else
        {
            Debug.Log($"Invlaid Move {attackingDice.value} not attacker");
            InvalidMove(targetEnemyDice.tileControl);
        }
    }

    internal async void CalculateAttackEnemyBase(Dice_Control attackingDice, Dice_Control targetEnemyBaseChild, List<TileControl> path)
    {
        if (attackingDice.value % 2 == 0)
        {
            await HopTo(path, attackingDice, true);
            int remainder = targetEnemyBaseChild.value - attackingDice.value;
            if (remainder < 0)
            {
                Debug.Log($"base destoryed and left {6 - -remainder}");
                DestroySingleDice(attackingDice);
                DeconstrucetBase(targetEnemyBaseChild, 6 - -remainder);
            }
            else if (remainder == 0)
            {
                Debug.Log($"base destoryed");
                DestroySingleDice(attackingDice);
                DeconstrucetBase(targetEnemyBaseChild, 6);
            }
            else
            {
                Debug.Log($"base now at {remainder}");
                StartCoroutine(targetEnemyBaseChild.ChangeDiceValue(remainder));
                DestroySingleDice(attackingDice);
            }
            gameControl.playerControl.TakenMove(path.Count);
            gameControl.AllowInput();
        }
        else
        {
            Debug.Log($"Invlaid Move {attackingDice.value} not attacker");
            InvalidMove(targetEnemyBaseChild.tileControl);
        }
        //each case either has destroy base, that allows input, or is invalid, and set allow input
    }

    internal void DeconstrucetBase(Dice_Control childDice, int remainingValue)
    {
        //Debug.Log($"destroying {childDice.gameObject.GetInstanceID()}");
        GameObject parentObject = childDice.gameObject.transform.parent.gameObject;
        Dice_Control parentDice = parentObject.GetComponent<Dice_Control>();
        parentDice.tileControl.SetDiceOnTile(parentDice);
        Destroy(childDice.gameObject);
        parentDice.isBase = false;
        parentDice.child = null;
        //Debug.Log($"setting {parentDice.gameObject.GetInstanceID()} to {remainingValue}");
        StartCoroutine(parentDice.ChangeDiceValue(remainingValue));
    }

    public void DestroySingleDice(Dice_Control diceToDestroy, bool removeTileDice = true)
    {
        if (removeTileDice) diceToDestroy.tileControl.RemoveDiceOnTile();
        diceToDestroy.SetDeselected();
        diceToDestroy.StopAllCoroutines();
        diceToDestroy.player.diceOwned.Remove(diceToDestroy.gameObject);
        Destroy(diceToDestroy.gameObject);
        //gameControl.AllowInput();//check here for if there is allow input issues
    }

    public Dice_Control CreateDiceAt(TileControl targetTile, Player player, bool returnDice = false)
    {
        GameObject newDice = Instantiate(Dice, new Vector3(targetTile.transform.position.x, 2.5f, targetTile.transform.position.z), Quaternion.identity);
        Dice_Control diceControl = newDice.GetComponent<Dice_Control>();

        diceControl.diceColour = player.playerColour;
        diceControl.player = player;
        diceControl.value = 1;
        diceControl.tileControl = targetTile;
        targetTile.SetDiceOnTile(diceControl);
        newDice.name = player.name;
        newDice.transform.rotation = Quaternion.Euler(GlobalVariables.data.SIDE_ONE.x, GlobalVariables.data.SIDE_ONE.y, GlobalVariables.data.SIDE_ONE.z);
        newDice.transform.rotation.Normalize();
        newDice.transform.SetParent(player.transform);
        player.diceOwned.Add(newDice);

        gameControl.AllowInput();

        return returnDice == true ? diceControl : null;
    }

    private void CreateBase(Dice_Control selectedDice, Dice_Control targetDice)
    {
        selectedDice.transform.position = new Vector3(targetDice.transform.position.x, 2.75f, targetDice.transform.position.z);
        selectedDice.tileControl.RemoveDiceOnTile();

        //bottom
        targetDice.isBase = true;
        targetDice.lowerDice = true;
        targetDice.tileControl.SetDiceOnTile(selectedDice);
        targetDice.child = selectedDice;

        //top
        selectedDice.isBase = true;
        selectedDice.lowerDice = false;
        selectedDice.tileControl = targetDice.tileControl;
        selectedDice.player.diceOwned.Remove(selectedDice.gameObject);
        selectedDice.gameObject.transform.SetParent(targetDice.gameObject.transform);
        selectedDice.gameObject.transform.localScale = new Vector3(1, 1, 1);

        gameControl.AllowInput();
    }
    //-----------------------------------------end dice actions

    public void UpdateBoardMeta(TileControl startingTile, TileControl targetTile, bool replaceTarget = true)
    {
        Dice_Control startingDice = startingTile.diceOnTile;
        Dice_Control targetDice = targetTile.diceOnTile;

        if (replaceTarget)
        {
            targetTile.SetDiceOnTile(startingDice);
            startingTile.RemoveDiceOnTile();
            startingDice.SetTile(targetTile);
        }
        else
        {
            targetTile.SetDiceOnTile(targetDice);//ensure target is not considered for pathfinding
            startingTile.RemoveDiceOnTile();
        }
    }

    public void InvalidMove(TileControl showInvalidMoveOnTile)
    {
        gameControl.AllowInput();
        showInvalidMoveOnTile.IndicateInvalidMove();
    }

    internal void SetupPlayerPieces(List<Player> players)
    {
        //notes maybe check gamemode with GlobalVariables.data.GAME_MODE
        Dice_Control lowerDice;
        Dice_Control upperDice;
        TileControl startingTile;
        foreach (var player in players)
        {
            startingTile = boardTiles[(int)player.startLocation.x, (int)player.startLocation.z];
            lowerDice = CreateDiceAt(startingTile, player, true);
            lowerDice.value = 6;
            lowerDice.transform.rotation = Quaternion.Euler(GlobalVariables.data.SIDE_SIX.x, GlobalVariables.data.SIDE_SIX.y, GlobalVariables.data.SIDE_SIX.z);
            lowerDice.transform.rotation.Normalize();
            lowerDice.transform.position = player.startLocation;

            upperDice = CreateDiceAt(startingTile, player, true);
            upperDice.value = 6;
            upperDice.transform.rotation = Quaternion.Euler(GlobalVariables.data.SIDE_SIX.x, GlobalVariables.data.SIDE_SIX.y, GlobalVariables.data.SIDE_SIX.z);
            upperDice.transform.rotation.Normalize();

            CreateBase(upperDice, lowerDice);
            //todo add in other dice for player eg if gamemode boosted, add 4 1 dice at corners
        }
    }

    private void GenerateBoard()
    {
        //Tile.transform.localScale = new Vector3(tileSize, 1, tileSize);
        GameObject tile;
        TileControl tileScript;

        for (float z = 0; z < depth * tileSize; z += tileSize)
        {
            for (float x = 0; x < width * tileSize; x += tileSize)
            {
                tile = Instantiate(Tile, new Vector3(x, tileSize, z), Quaternion.identity);
                tileScript = tile.GetComponent<TileControl>();
                tileScript.tileIndex = new Vector2Int( (int)x, (int)z );              
                tileScript.tileColour = WhiteTile == true ? WhiteMaterial : BlackMaterial;
                tileScript.SetIsEmptyTile();
                tileScript.SetValidPathfing();
                tile.name = tileName;
                tile.transform.SetParent(gameObject.transform);//set the new tile to the BoardGrid

                boardTiles[tileScript.tileIndex.x, tileScript.tileIndex.y] = tileScript;
                WhiteTile = !WhiteTile;/*alternate when to add, black is false first*/
            }
            if (width % 2 == 0)
                WhiteTile = !WhiteTile;/*for diagonal design*/
        }
    }

    public void SetToDefault()
    {
        //remove board tiles
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(KickOffSetup());
    }
}
