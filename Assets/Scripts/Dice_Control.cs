using System.Collections;
using UnityEngine;

public class Dice_Control : MonoBehaviour
{
    //meta
    public Rigidbody rb;
    public Renderer cubeRenderer;
    public Material diceColour;
    public Material selectedDiceColour;
    public Player player;
    public TileControl tileControl;
    public int value;
    public bool isBase = false;
    public bool lowerDice = true;
    public Dice_Control child = null;
    public bool selected = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cubeRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        diceColour = GetComponent<Renderer>().material = diceColour;
    }

    //private void Update()
    //{
    //    if(selected)
    //    {
    //        //run animation for slected, //need parent to run relative
    //        if (showSelected)
    //        {
    //            StartCoroutine(ShowSelectedAnimation()); here
    //        }
    //    }
    //}

    //private void FixedUpdate()
    //{       
    //    ////replace with coroutines https://youtu.be/5L9ksCs6MbE?t=192
    //    //if (selected)
    //    //{
    //    //    if (Input.anyKeyDown && !moving)
    //    //    {
    //    //        switch (Input.inputString)
    //    //        {
    //    //            case "h": moveHome(); break;
    //    //            case "w": moveForward(); break;
    //    //            case "a": moveLeft(); break;
    //    //            case "s": moveBack(); break;
    //    //            case "d": moveRight(); break;
    //    //        }
    //    //    }
    //    //}
    //    //if (moving)
    //    //{
    //    //    // Distance moved equals elapsed time times speed..
    //    //    float distCovered = (Time.time - startTime) * moveSpeed;

    //    //    // Fraction of journey completed equals current distance divided by total distance.
    //    //    float fractionOfJourney = distCovered / journeyLength;

    //    //    transform.position = Vector3.Lerp(origin, destination, fractionOfJourney);
    //    //    if (fractionOfJourney >= 1)
    //    //    {
    //    //        //spap to nearest tile. todo make sure if in the center
    //    //        transform.position = new Vector3((float)Mathf.Round(destination.x), 
    //    //            transform.position.y, 
    //    //            (float)Mathf.Round(destination.z));
    //    //        moving = false;
    //    //        //todo, set dice x,z and tile not empty
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    //free movement not applicable anymore
    //    //    //transform.Translate(moveSpeed * GetX() * Time.deltaTime, 0f, moveSpeed * GetZ() * Time.deltaTime);
    //    //}
    //}

    public void SetSelected()
    {
        selected = true;
        StartCoroutine(ShowSelectedAnimation());
    }
    public void SetDeselected()
    {
        selected = false;
        StartCoroutine(DeSelectedAnimation());
    }

    public void SetTile(TileControl onTile) => this.tileControl = onTile;
    

    public void RemoveTile() => this.tileControl = null;
    

    public IEnumerator DeSelectedAnimation()
    {
        //workaround untill animations are done
        if (lowerDice == true && GlobalVariables.data.SHOW_DICE_INFLATE_ANIMATION)
        {
            Vector3 scale = new Vector3();
            for (float j = .5f; j > .35f; j -= .05f)
            {
                scale.x = j;
                scale.y = j;
                scale.z = j;
                transform.localScale = scale;
            }
            yield return new WaitForSeconds(GlobalVariables.data.FLASH_SELECTED_SPEED);
            for (float j = .35f; j < .5f; j += .05f)
            {
                scale.x = j;
                scale.y = j;
                scale.z = j;
                transform.localScale = scale;
            }
            scale.x = .5f;
            scale.y = .5f;
            scale.z = .5f;
            transform.localScale = scale;
        }
    }

    public IEnumerator ShowSelectedAnimation()
    {
        bool showInflate = GlobalVariables.data.SHOW_DICE_INFLATE_ANIMATION;
        float flashSpeed = GlobalVariables.data.FLASH_SELECTED_SPEED;

        Vector3 scale = new Vector3();
        while (selected)
        {
            cubeRenderer.material = selectedDiceColour;
            if (child != null)
            {
                child.cubeRenderer.material = child.selectedDiceColour;
                //todo figure out why the scale is not being set to 1 for the upper base dice
                //patch to prevent upper dice from being small in some cases.
                //child.transform.localScale = new Vector3(1f, 1f, 1f);
                //child.selected = false;
            }

            if (lowerDice == true)
            {   
                if (showInflate && lowerDice)
                for (float i = 0.5f; i < .6f; i += .01f)
                {
                    scale.x = i;
                    scale.y = i;
                    scale.z = i;
                    transform.localScale = scale;
                }
                yield return new WaitForSeconds(flashSpeed);
                if (showInflate && lowerDice)
                for (float j = .6f; j > .5f; j -= .01f)
                {
                    scale.x = j;
                    scale.y = j;
                    scale.z = j;
                    transform.localScale = scale;
                }
                cubeRenderer.material = diceColour;
                if (child != null) child.cubeRenderer.material = child.diceColour;
                yield return new WaitForSeconds(flashSpeed);               
            } 
            //else
            //{
            //    transform.localScale = new Vector3(1f, 1f, 1f);                
            //    selected = false;
            //}
            
        }
        cubeRenderer.material = diceColour;
        if (child != null) child.cubeRenderer.material = child.diceColour;
    }

    public IEnumerator FlashForNewTurn()
    {
        Color diceColor = diceColour.color;

        cubeRenderer.material.SetColor("_Color", Color.white);
        yield return new WaitForSeconds(.1f);
        cubeRenderer.material.SetColor("_Color", diceColor);
        yield return new WaitForSeconds(.2f);

        cubeRenderer.material.SetColor("_Color", Color.white);
        yield return new WaitForSeconds(.1f);
        cubeRenderer.material.SetColor("_Color", diceColor);
        yield return new WaitForSeconds(.2f);

        cubeRenderer.material.SetColor("_Color", Color.white);
        yield return new WaitForSeconds(.1f);
        cubeRenderer.material.SetColor("_Color", diceColor);
        yield return null;
        
    }

    public IEnumerator ChangeDiceValue(int newValue)
    {
        //workaround as kept randommly stopping at a rotaiton -> need animation
        float speed = 1f;
        switch (value)
        {
            case 1: rb.AddForce(transform.up * speed, ForceMode.VelocityChange); break;
            case 2: rb.AddForce(transform.forward * speed, ForceMode.VelocityChange); break;
            case 3: rb.AddForce(-transform.right * speed, ForceMode.VelocityChange); break;
            case 4: rb.AddForce(-transform.forward * speed, ForceMode.VelocityChange); break;
            case 5: rb.AddForce(transform.right * speed, ForceMode.VelocityChange); break;
            case 6: rb.AddForce(-transform.up * speed, ForceMode.VelocityChange); break;
        }

        Vector3 currentRaw = transform.rotation.eulerAngles;
        Quaternion target = new Quaternion();
        switch(newValue)
        {
            /*
            case 1: target = Quaternion.Euler(0, 0, 0); break;
            case 2: target = Quaternion.Euler(270, 0, 0); break;
            case 3: target = Quaternion.Euler(0, 0, 270); break;
            case 4: target = Quaternion.Euler(90, 0, 0); break;
            case 5: target = Quaternion.Euler(0, 0, 90); break;
            case 6: target = Quaternion.Euler(180, 0, 0); break;
            */
            case 1: target = Quaternion.Euler(GlobalVariables.data.SIDE_ONE.x, GlobalVariables.data.SIDE_ONE.y, GlobalVariables.data.SIDE_ONE.z); break;
            case 2: target = Quaternion.Euler(GlobalVariables.data.SIDE_TWO.x, GlobalVariables.data.SIDE_TWO.y, GlobalVariables.data.SIDE_TWO.z); break;
            case 3: target = Quaternion.Euler(GlobalVariables.data.SIDE_THREE.x, GlobalVariables.data.SIDE_THREE.y, GlobalVariables.data.SIDE_THREE.z); break;
            case 4: target = Quaternion.Euler(GlobalVariables.data.SIDE_FOUR.x, GlobalVariables.data.SIDE_FOUR.y, GlobalVariables.data.SIDE_FOUR.z); break;
            case 5: target = Quaternion.Euler(GlobalVariables.data.SIDE_FIVE.x, GlobalVariables.data.SIDE_FIVE.y, GlobalVariables.data.SIDE_FIVE.z); break;
            case 6: target = Quaternion.Euler(GlobalVariables.data.SIDE_SIX.x, GlobalVariables.data.SIDE_SIX.y, GlobalVariables.data.SIDE_SIX.z); break;
        }        

        //while loop ends before the if can take effect -> 
        while(currentRaw != target.eulerAngles)
        {
            //works great, except there is at .1 error so need to snap, or have range
            if (currentRaw != target.eulerAngles)
            {
                transform.Translate(GetX() * Time.deltaTime, 0f, GetZ() * Time.deltaTime);

                currentRaw = Quaternion.RotateTowards(transform.rotation, target, 10f).eulerAngles;
                //currentRaw.x = Mathf.Round(currentRaw.x);
                //currentRaw.y = Mathf.Round(currentRaw.y);
                //currentRaw.z = Mathf.Round(currentRaw.z);
                ////currentRaw.x -= currentRaw.x % 5;
                ////currentRaw.y -= currentRaw.y % 5;
                ////currentRaw.z -= currentRaw.z % 5;
                transform.eulerAngles = currentRaw;
                //print("current Rotation: " + transform.eulerAngles + " -> " + target.eulerAngles + " : " + value + " to " + newValue);

                yield return null;
            }
        }
        transform.rotation.Normalize();
        //rb.useGravity = true;
        yield return new WaitForSeconds(Time.deltaTime);
        //print("Done Rotating: " + transform.eulerAngles + " -> " + target.eulerAngles + " : " + value + " to " + newValue);
        value = newValue;
        GameObject.Find("Game Control").GetComponent<GameControl>().AllowInput();
    }

    //public IEnumerator MoveTo(float startTime, Vector3 origin, Vector3 destination, float journeyLength)
    //{
    //    float distCovered;
    //    float fractionOfJourney = 0;

    //    while (fractionOfJourney <= 1) { 
    //        distCovered = (Time.time - startTime) * 5f;

    //        fractionOfJourney = distCovered / journeyLength;
    //        transform.position = Vector3.Lerp(origin, destination, fractionOfJourney);

    //        if (fractionOfJourney >= 1)
    //        {
    //            transform.position = new Vector3((float)Mathf.Round(destination.x),
    //                transform.position.y,
    //                (float)Mathf.Round(destination.z));
    //        } else
    //        {
    //            yield return new WaitForSeconds(Time.deltaTime);
    //        }
    //    }
    //    GameObject.Find("Game Control").GetComponent<GameControl>().AllowInput();
    //}

    //public IEnumerator MoveTo()
    //{
    //    float distCovered;
    //    float fractionOfJourney = 0;

    //    while (fractionOfJourney < 1) { 
    //        // Distance moved equals elapsed time times speed..
    //        distCovered = (Time.time - startTime) * 5f;

    //        // Fraction of journey completed equals current distance divided by total distance.
    //        fractionOfJourney = distCovered / journeyLength;
    //        //print(journeyLength + " " + distCovered + " " + fractionOfJourney);
    //        transform.position = Vector3.Lerp(origin, destination, fractionOfJourney);
    //        if (fractionOfJourney >= 1)
    //        {
    //            //spap to nearest tile. todo make sure if in the center
    //            transform.position = new Vector3((float)Mathf.Round(destination.x),
    //                transform.position.y,
    //                (float)Mathf.Round(destination.z));
    //            //todo, set dice x,z and tile not empty
    //        } else
    //        {
    //            //basically keep running until condition above is met
    //            yield return new WaitForSeconds(Time.deltaTime);
    //        }
    //    }
    //    GameObject.Find("Game Control").GetComponent<GameControl>().AllowInput();
    //}

    //public void LeapTo(TileControl newTile, bool relative = true)
    //{
    //    //https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html

    //    //this ensures that the moving piece will not smash into something
    //    Vector3 moveAboveBoard = transform.position;
    //    moveAboveBoard.y = 3f;
    //    transform.position = moveAboveBoard;

    //    startTime = Time.time;
    //    origin = transform.position;
    //    destination = relative == true ? origin + newTile.transform.position : newTile.transform.position;
    //    destination.y = 3f;
    //    // Calculate the journey length for % of movement per update
    //    journeyLength = Vector3.Distance(origin, destination);
    //    //print("End Moving from position " + origin + " too " + destination);
    //    StartCoroutine(MoveTo());
    //    tileControl.RemoveDiceOnTile();
    //    tileControl = newTile;
    //    tileControl.SetDiceOnTile(this);
    //}

    public void StopAnimation()
    {
        StopAllCoroutines();
    }

    //private void JumpUp()
    //{
    //    rb.AddForce(transform.up * thrust * scale, ForceMode.VelocityChange);
    //}

    private static float GetZ()
    {
        return Input.GetAxis("Vertical");
    }

    private static float GetX()
    {
        return Input.GetAxis("Horizontal");
    }
}
