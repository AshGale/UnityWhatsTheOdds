using System.Collections;
using UnityEngine;

public class TileControl : MonoBehaviour
{
    public bool validPathfindingTile = true;
    public bool isEmptyTile = true;

    public Dice_Control diceOnTile;
    public Material tileColour;
    Color invalid = Color.red;
    Color possible = Color.yellow;
    public Vector2Int tileIndex;
    public int pathValue;
    Renderer tileRenderer;

    private void Start()
    {
        pathValue = -1;
        tileRenderer = GetComponent<Renderer>();
        tileRenderer.material = tileColour;
    }

    public void SetPathValue(int pathValue) => this.pathValue = pathValue;

    public void SetValidPathfing() => this.validPathfindingTile = true;

    public void SetCanNotPassThrough() => this.validPathfindingTile = false;

    public void SetIsEmptyTile() => this.isEmptyTile = true;

    public void SetHasPieceOnTile() => this.isEmptyTile = false;

    public void ResetPathValue() => pathValue = -1;

    public void SetDiceOnTile(Dice_Control diceOnTile)
    {
        this.diceOnTile = diceOnTile;
        SetHasPieceOnTile();
        SetCanNotPassThrough();
    }

    public void RemoveDiceOnTile()
    {
        this.diceOnTile = null;
        SetIsEmptyTile();
        SetValidPathfing();
    }

    public void ShowPossibleMove() =>  StartCoroutine(IndicateColor(possible));
    public void IndicateInvalidMove() =>  StartCoroutine(IndicateColor(invalid));

    private IEnumerator IndicateColor(Color indicationColor)
    {
        tileRenderer.material.color = indicationColor;
        float start = Time.time;
        float elapsed = 0;
        while (elapsed < 1f)
        {
            // calculate how far through we are
            elapsed = Time.time - start;
            float normalisedTime = Mathf.Clamp(elapsed / 1f, 0f, 1f);
            tileRenderer.material.color = Color.Lerp(invalid, tileColour.color, normalisedTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
