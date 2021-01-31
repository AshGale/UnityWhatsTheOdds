using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*https://www.studica.com/blog/how-to-create-a-singleton-in-unity-3d */
public class GlobalVariables : MonoBehaviour
{

    /* Note
     * ToDo List:
     * Base creation, not next to other base
     * 
     * Add in indicator for ammount of moves as banner or item at the bottom (done)
     * - make new Prefab for dice and moves indicator, to be placed on your side of board.
     * - show number of moves in green dice on player rack
     * 
     * Show the Valid moves based on ammount of moves display
     * - will requre you too add in layer, or change material of the tiles OR indicate invalid move
     * 
     * 
     * Change the creation to a dictionary que of eg 12 dice per player
     * - maybe indicate how many dice on eahc player in ui type thing
     * - have number of dice available as global variable
     * 
     * Make a Dice_Controller and make dice a bare bones object
     * - much refactoring, may have to make some other objects sciptable object if no update is used
     * - remove lots of code from the board controller
     * - make class for the pathfinding stuff
     * - make pathfinding A* by adding distance to target
     * 
     * Make a User Input Controller for mouse, and keyboard inputs. 
     * 
     * Replace Path with struct
     * 
    * BUGS:
    * can make a quick move after you have done your last. this is due chance dice value is at the end of the change dice value. 
    *  - update it to be a async await funciton, to ensure syncronus logic.
    * 
    * 
   */

    public static GlobalVariables data = null;
    //Board
    public float TILE_SIZE = 1;
    public string TILE_NAME = "Tile"; 
    public string PLAYER1_NAME = "TeamOne"; 
    public string PLAYER2_NAME = "TeamTwo"; 
    public string PLAYER3_NAME = "TeamThree"; 
    public string PLAYER4_NAME = "TeamFour";

    //Note needs to bee synced with NewGameScreen colour dropdown values
    public string PLAYER1_COLOUR = "Blue";
    public string PLAYER2_COLOUR = "Red";
    public string PLAYER3_COLOUR = "Teal";
    public string PLAYER4_COLOUR = "Purple";

    public string UI_ACTIONS_TEXT = "Actions: ";
    public string UI_TURN_TEXT = "Team: ";

    public int WIDTH = 8;
    public int DEPTH = 8;
    public int MOVES_FOR_BASE = 3; 
    public int WORKER_INCOME_1 = 1;
    public int WORKER_INCOME_3 = 2;
    public int WORKER_INCOME_5 = 3;
    //Dice

    //player
    public Vector3 PLAYER_1_START = new Vector3(2, 2, 2);
    public Vector3 PLAYER_2_START = new Vector3(2, 2, 5);
    public Vector3 PLAYER_3_START = new Vector3(5, 2, 5);
    public Vector3 PLAYER_4_START = new Vector3(5, 2, 2);

    public Vector3 PLAYER_1_CAMERA_DEFAULT = new Vector3(65, 360, 0);
    public Vector3 PLAYER_2_CAMERA_DEFAULT = new Vector3(65, 90, 0);
    public Vector3 PLAYER_3_CAMERA_DEFAULT = new Vector3(65, 180, 0);
    public Vector3 PLAYER_4_CAMERA_DEFAULT = new Vector3(65, 270, 0);

    /*public Quaternion SIDE_ONE = Quaternion.Euler(new Vector3(90, 0, 0));
    public Quaternion SIDE_TWO = Quaternion.Euler(new Vector3(180, 0, 0));
    public Quaternion SIDE_THREE = Quaternion.Euler(new Vector3(0, 0, 270));
    public Quaternion SIDE_FOUR = Quaternion.Euler(new Vector3(0, 0, 0));
    public Quaternion SIDE_FIVE = Quaternion.Euler(new Vector3(0, 0, 90));
    public Quaternion SIDE_SIX = Quaternion.Euler(new Vector3(270, 0, 0));*/

    public Quaternion SIDE_ONE = Quaternion.Euler(270, 0, 0);
    public Quaternion SIDE_TWO = Quaternion.Euler(0, 0, 0);
    public Quaternion SIDE_THREE = Quaternion.Euler(0, 0, 90);
    public Quaternion SIDE_FOUR = Quaternion.Euler(180, 0, 0);
    public Quaternion SIDE_FIVE = Quaternion.Euler(0, 0, 270);
    public Quaternion SIDE_SIX = Quaternion.Euler(90, 0, 0);

    public bool SHOW_DICE_INFLATE_ANIMATION = true;
    public bool SHOW_FLASH_START_TURN = true;

    public int CAMERA_ROTATION_SPEED = 60;
    public float SCROLL_SPEED = .5f;
    public float FLASH_SELECTED_SPEED = .5f;
    public float BASE_SPAWN_DISTANCE = 1.3f;
    public float DICE_SIZE = 0.5f;
    public float MOVE_SPEED = 5;
    public float THRUST_SPEED = 2;
    public int NUMER_OF_PLAYERS = 2;
    public float BOARD_CLEARANCE = 2.2f;
    public float TILE_CLEARANCE = 1.8f;
    public float HOP_DELAY_TIME = .2f;
    public float CURRENT_ZOOM = 2.5f;
    public float MAX_ZOOM = 2.5f;

    void Awake()
    {
        if(data == null)       
            data = this;
        else if (data != this)
            Destroy(gameObject);
    }
    /*
     * game modes ideas:
     * base gives +3 for each base, dice loose 1 for each move, not other income
     * dice role in the orientation they will move ie 1 moves 2 in any direction becomes 6
     * game mode for die, on no moves this turn
     * add 2v2 mode
     * 
     * Full turnbase game, where get one move per turn.
     * - odd units just don't attack, and don't generate income
     * - role dice to see who goes first, then that person roles, again until another number is roled
     * - 2 player game, role 1 dice to see who goes, odd player 1 goes, evens player 2 goes ie 1, 3, 5 player on moves
     * - 3 player game, role 1 dice, 1, 5 player onw, 2, 4 player two, 3, 6 player three
     * - 4 player game, role 4 dice and well, 4-8 9,10,11,13,14 15-19 20-24 => nb 12 is role again
     * 
     * seems 2 player with same name works really well
     * 
     */
}
