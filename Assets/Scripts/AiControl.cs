using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiControl : MonoBehaviour
{

    public GameControl gameControl;

    public Dice_Control target;
    public Player player;
    int SoldierValue = 2;
     

    public void EasyAi()
    {
        Debug.Log($"In easy AI");
        player = gameControl.playerControl.activePlayer;
        //yield return new WaitForSeconds(.5f);

        if(target == null)
        {
            //find nearest enemy unit
            // - use pathfinding logic to get adjacent to all pieces
            // - if any of those tile have a enemy pice on it, then set as target.
            //   else repeat until you find enemy. forgit, is there is no other pieces on the board
        }

        //while actions > 0 && PlayerHasSoldier = false

        if (PlayerHasSoldier())
        {
            //move towards target
        } else
        {
            //if player has worker
            //add too worker, 
            //else create dice
        }

    }

    public bool PlayerHasSoldier()
    {
        int dice_value;
        Dice_Control dice_Control;

        foreach ( GameObject piece in player.diceOwned) {
            dice_Control = piece.GetComponent<Dice_Control>();
            dice_value = dice_Control.value;
            if (dice_Control.isBase)
            {
                //don't care
            } else
            {
                if (dice_value >= SoldierValue)
                {
                    return true;
                }
            }
        }
        return false;
    }

}
