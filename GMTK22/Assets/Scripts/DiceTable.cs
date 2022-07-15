using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceTable : MonoBehaviour
{
    struct DiceSlot
    {
        public Dice dice;
        public Vector2 position;
    }

    public int dicePerRow;
    public int dicePerColumn;
    public float diceSpacing;
    public GameObject dicePrefab;

    private DiceSlot[,] diceGrid;
    private float gridHeight;
    private float gridWidth;
    private float diceWidth;
    private float diceHeight;

    // Start is called before the first frame update
    void Start()
    {
        diceGrid = new DiceSlot[dicePerRow, dicePerColumn];

        CreateDice();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0.0f;

        // If dice is clicked roll it
        if (Input.GetMouseButtonDown(0))
        {
            bool diceFound = false;
            for (int i = 0; i < dicePerRow; i++)
            {
                for (int j = 0; j < dicePerColumn; j++)
                {
                    Dice dice = diceGrid[i, j].dice;
                    if (dice.IsPointOnDice(mouseWorldPosition))
                    {
                        dice.RollDice();
                        Debug.Log("Roll Dice: " + dice.gameObject.name);
                        diceFound = true;
                        break;
                    }
                }
                
                if (diceFound)
                {
                    break;
                }
            }
        }
        else  // If mouse is over dice then highlight it
        {
            for (int i = 0; i < dicePerRow; i++)
            {
                for (int j = 0; j < dicePerColumn; j++)
                {
                    Dice dice = diceGrid[i, j].dice;
                    if (dice.IsPointOnDice(mouseWorldPosition))
                    {
                        dice.HighlightDice();
                    }
                    else
                    {
                        dice.UnhighlightDice();
                    }
                }
            }
        }
    }

    void CreateDice()
    {
        diceWidth = dicePrefab.GetComponent<BoxCollider2D>().size.x;
        diceHeight = dicePrefab.GetComponent<BoxCollider2D>().size.y;
        gridWidth = dicePerRow * diceWidth + diceSpacing * (dicePerRow - 1);
        gridHeight = dicePerColumn * diceHeight + diceSpacing * (dicePerColumn - 1);

        Vector2 topLeftPos = new Vector2(-gridWidth * 0.5f + diceWidth * 0.5f, gridHeight * 0.5f - diceHeight * 0.5f);
        Vector2 diceSeparation = new Vector2(diceWidth + diceSpacing, -diceHeight - diceSpacing);

        for (int i = 0; i < dicePerRow; i++)
        {
            for (int j = 0; j < dicePerColumn; j++)
            {
                Vector2 dicePos = topLeftPos + new Vector2(i*diceSeparation.x, j*diceSeparation.y);
                GameObject diceObject = Instantiate(dicePrefab, dicePos, Quaternion.identity);
                diceGrid[i, j].dice = diceObject.GetComponent<Dice>();
                diceGrid[i, j].position = dicePos;
            }
        }
    }
}
