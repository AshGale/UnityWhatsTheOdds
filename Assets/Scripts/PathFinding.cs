using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GetAdjacentTilesType
{
    All, AllEmpty, AllUnprocessed, AllProcessed, AllEmptyAndUnprocessed, AllFriendlyOutposts
}

public class PathFinding : MonoBehaviour
{

    readonly List<Vector2Int> adjacent = new List<Vector2Int> { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    public GameControl gameControl;
    private TileControl[,] boardTiles;
    private int width;
    private int depth;

    private void Start()
    {
        UpdatePathfindingBoundry(GlobalVariables.data.WIDTH, GlobalVariables.data.DEPTH);
    }

    public void UpdatePathfindingBoundry(int w, int d)
    {
        width = w;
        depth = d;
    }

    public List<TileControl> FindPathToNearestEnemy(TileControl sorceTile, Player currentPlayer)
    {
        List<TileControl> adjacentTiles = new List<TileControl>();
        List<TileControl> adjacentTilesBuffer = new List<TileControl>();
        boardTiles = RefreshBoardTiles(); 

        Debug.Log($"Looking for enemy near {sorceTile.tileIndex} who isn't player {currentPlayer.name}");

        bool enemyFound = false;
        TileControl target = sorceTile;// for value not set compilaiton error
        int pathCost = 0;
        adjacentTiles.Add(sorceTile);//first itteration has source tile, then add each valid tile for next loop
        while (!enemyFound)
        {
            foreach (TileControl tile in adjacentTiles)
            {
                if (tile.diceOnTile && tile.diceOnTile.player.name != currentPlayer.name)                
                {
                    //adjacent tile, had dice on it, is it an ememy dice                   
                    enemyFound = true;
                    //gameControl.playerControl.activePlayer.ai.target = target.diceOnTile;//could be better way
                    tile.SetPathValue(pathCost);
                    target = tile;
                    Debug.Log($"Found enemy {target.diceOnTile.currentValue} {target.diceOnTile.player.name} at {target.tileIndex}");
                    break;//for
                    //may have but that dosen't add in new adjacent tiles when there is dice in the way
                }
                else
                {
                    //tile is where you add in addional tiles to consider for pathfinding
                    tile.SetPathValue(pathCost);
                    foreach (TileControl validTile in GetAjacentTiles(tile, GetAdjacentTilesType.AllUnprocessed))
                    {
                        if(validTile.diceOnTile != null && validTile.diceOnTile.player.playerName == currentPlayer.playerName)
                        {
                            //Debug.Log($"skipped own dice tile at {validTile.tileIndex}");
                        } else 
                        adjacentTilesBuffer.Add(validTile);//for next loop
                        //Debug.Log($"added {validTile.tileIndex} to tile buffer");
                    }
                }
            }
            //setup for next loop, assign the next adjacent list form buffer, and clear buffer, incrment pathCost
            if (!enemyFound)
            {
                if (adjacentTilesBuffer.Count == 0)
                {
                    //should never get here, while there is an ememy on the board
                    Debug.Log($"No more tiles to find, Invalid path to target");
                    break;
                }
                else
                {
                    pathCost++;
                    //keep looping untill you have an enemy
                    //A* this is where you would check for closest to target, targetX-tileX + targetZ-tileZ as positive
                    adjacentTiles = adjacentTilesBuffer.ToList();//due to C#ness need to specify new list
                    adjacentTilesBuffer.Clear();
                }
            }
        }
        //always return somepath, otherwise there will be no enemies. 
        //Debug.Log($"Path to target exists, retriving. . .");            
        return GetPath(target);

    }

    private TileControl[,] RefreshBoardTiles()
    {
        //sorceTile.BroadcastMessage("ResetPathValue");//reset all tiles to a -1 value// currently not broadcasying, current issu, 
        boardTiles = gameControl.boardControl.GetBoardTiles();
        for ( int w =0; w< width; w++)
        {
            for (int d = 0; d < depth; d++)
            {
                boardTiles[w, d].ResetPathValue();
            }
        }
        return boardTiles;
    }

    private List<TileControl> GetPath(TileControl targetTile, bool getOnlyLimit = false, int limit = 100)
    {
        List<TileControl> path = new List<TileControl>() { targetTile };

        List<TileControl> tileBuffer;
        TileControl shortestTile = targetTile;
        bool gettingPath = true;
        while (gettingPath)
        {
            tileBuffer = GetAjacentTiles(shortestTile, GetAdjacentTilesType.AllProcessed);
            foreach (var tile in tileBuffer)
            {
                if (tile.pathValue == 0)
                {
                    //Debug.Log($"at target {targetTile.tileIndex} = {tile.tileIndex}");
                    path.Insert(0, tile);
                    gettingPath = false;
                    shortestTile = tile;
                    return path;
                }
                else if (tile.pathValue < shortestTile.pathValue)
                {
                    Debug.Log($"closer tile found {tile.tileIndex}");
                    shortestTile = tile;
                }
            }
            path.Insert(0, shortestTile);//at start
            shortestTile = path.First();
            //was going restrict path for nearest enemy, but would no good way to get target
            if (getOnlyLimit && path.Count() > limit)
            {
                //Debug.Log($"move as far as you can towards {targetTile}");
                return path;
            }
        }
        return null;
    }

    public List<TileControl> GetAjacentTiles(TileControl tile, GetAdjacentTilesType type, Player player = null)
    {
        //Debug.Log($"Getting AjacentTiles to {tile.tileIndex} {type}");
        List<TileControl> adjacentTiles = new List<TileControl>();
        boardTiles = gameControl.boardControl.GetBoardTiles();//latest
        Vector2Int nextIndex;
        TileControl nextTile;

        foreach (Vector2Int direction in adjacent)
        {
            nextIndex = tile.tileIndex + direction;
            //Debug.Log($" - {tile.tileIndex}+{direction}={nextIndex}");
            if (nextIndex.x < 0 || nextIndex.y < 0 || nextIndex.x >= width || nextIndex.y >= depth)
            {
                //Debug.Log($"Skipped path tile: {nextIndex} out of bounds");
            }
            else
            {
                nextTile = boardTiles[nextIndex.x, nextIndex.y];
                //logic to consider what tiles to add around 'tile'
                // - add only empty tiles, that have not path value, and has not been processed before
                // - add when value is -1 ie not been processed before
                // - add when there is dice on tile, and diceOnTile isBase
                // - skip when outside board if (nextIndex.x < 0 || nextIndex.y < 0 || nextIndex.x >= width || nextIndex.y >= depth)
                switch (type)
                {
                    case GetAdjacentTilesType.All:
                        {
                            adjacentTiles.Add(nextTile);
                            break;
                        }
                    case GetAdjacentTilesType.AllEmpty:
                        {
                            if (nextTile.isEmptyTile)
                                adjacentTiles.Add(nextTile);
                            break;
                        }
                    case GetAdjacentTilesType.AllUnprocessed:
                        {
                            //NB ensure used appropriately BroadcastMessage("ResetPathValue");
                            if (nextTile.pathValue == -1)
                                adjacentTiles.Add(nextTile);
                            break;
                        }
                    case GetAdjacentTilesType.AllProcessed:
                        {
                            if (nextTile.pathValue != -1)
                                adjacentTiles.Add(nextTile);
                            break;
                        }
                    case GetAdjacentTilesType.AllEmptyAndUnprocessed:
                        {
                            if (nextTile.pathValue == -1 && nextTile.isEmptyTile)
                                adjacentTiles.Add(nextTile);
                            break;
                        }
                    case GetAdjacentTilesType.AllFriendlyOutposts:
                        {
                            if (nextTile.isEmptyTile == false && nextTile.diceOnTile.isBase)
                            {
                                if (nextTile.diceOnTile.player.playerName == player.playerName)
                                    adjacentTiles.Add(nextTile);
                            }
                            break;
                        }
                    default: break;
                }
            }
        }
        return adjacentTiles;
    }
}
