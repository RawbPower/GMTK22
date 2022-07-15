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

    private int[,] diceGrid;
    private float gridHeight;
    private float gridWidth;
    private float diceWidth;
    private float diceHeight;

    // Start is called before the first frame update
    void Start()
    {
        diceGrid = new int[dicePerRow, dicePerColumn];

        CreateDice();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            for (int j = 0; j < dicePerRow; j++)
            {
                Vector2 dicePos = topLeftPos + new Vector2(i*diceSeparation.x, j*diceSeparation.y);
                Instantiate(dicePrefab, dicePos, Quaternion.identity);
            }
        }
    }
}
