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
    public int matchLength = 3;
    public GameObject dicePrefab;
    public float effectedDiceRollDelay;

    private DiceSlot[,] diceGrid;
    private float gridHeight;
    private float gridWidth;
    private float diceWidth;
    private float diceHeight;
    private Dice[] effectedDice;

    // Start is called before the first frame update
    void Start()
    {
        diceGrid = new DiceSlot[dicePerRow, dicePerColumn];

        CreateDice();
    }

    // Update is called once per frame
    void Update()
    {
        if (!AreAnyDiceRolling())
        {
            if (!AreAnyDiceMatched())
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0.0f;

                Dice selectedDice = null;
                Vector2Int hoverDiceIndex = new Vector2Int(0, 0);
                for (int i = 0; i < dicePerRow; i++)
                {
                    for (int j = 0; j < dicePerColumn; j++)
                    {
                        Dice dice = diceGrid[i, j].dice;
                        if (dice.IsPointOnDice(mouseWorldPosition))
                        {
                            dice.HighlightDice();
                            selectedDice = diceGrid[i, j].dice;
                            hoverDiceIndex = new Vector2Int(i, j);
                        }
                        else
                        {
                            dice.UnhighlightDice();
                        }
                    }
                }

                effectedDice = new Dice[0];
                if (selectedDice)
                {
                    DiceEffect diceEffect = selectedDice.GetDiceEffect();
                    effectedDice = GetEffectDice(hoverDiceIndex, diceEffect);
                    foreach (Dice dice in effectedDice)
                    {
                        dice.HighlightDice();
                    }
                }

                // If dice is clicked roll it
                if (Input.GetMouseButtonDown(0))
                {
                    if (selectedDice)
                    {
                        selectedDice.RollDice();

                        StartCoroutine(RollEffectedDice());
                    }
                }
            }

            if (!AreAnyDiceRolling())
            {
                Dice[] matchedDice = DetectMatches();

                foreach (Dice matchedDie in matchedDice)
                {
                    matchedDie.MatchDice();
                }
            }
        }
    }

    IEnumerator RollEffectedDice()
    {
        yield return new WaitForSeconds(effectedDiceRollDelay);
        foreach (Dice dice in effectedDice)
        {
            dice.RollDice();
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

    public bool AreAnyDiceRolling()
    {
        bool areAnyDiceRolling = false;
        for (int x = 0; x < dicePerRow; x++)
        {
            for (int y = 0; y < dicePerColumn; y++)
            {
                if (diceGrid[x,y].dice.rolling)
                {
                    areAnyDiceRolling = true;
                    break;
                }
            }
        }

        return areAnyDiceRolling;
    }

    public bool AreAnyDiceMatched()
    {
        bool areAnyDiceMatched = false;
        for (int x = 0; x < dicePerRow; x++)
        {
            for (int y = 0; y < dicePerColumn; y++)
            {
                if (diceGrid[x, y].dice.IsMatched())
                {
                    areAnyDiceMatched = true;
                    break;
                }
            }
        }

        return areAnyDiceMatched;
    }

    private Dice[] DetectMatches()
    {
        List<Dice> matchedDice = new List<Dice>();
        for (int x = 0; x < dicePerRow; x++)
        {
            for (int y = 0; y < dicePerColumn; y++)
            {
                Dice dice = diceGrid[x, y].dice;
                int matchNumber = dice.GetNumber();

                // Find Horizontal Matches
                if (x + matchLength - 1 < dicePerRow && !dice.horMatched)
                {
                    bool foundMatch = true;
                    for (int i = 1; i < matchLength; i++)
                    {
                        if (diceGrid[x+i,y].dice.GetNumber() != matchNumber)
                        {
                            foundMatch = false;
                            break;
                        }
                    }

                    if (foundMatch)
                    {
                        Debug.Log("Horizontal Match Found: " + matchNumber);
                        for (int i = 0; i < dicePerRow; i++)
                        {
                            if (i < matchLength)
                            {
                                if (!diceGrid[x + i, y].dice.IsMatched())
                                {
                                    matchedDice.Add(diceGrid[x + i, y].dice);
                                }
                                diceGrid[x + i, y].dice.horMatched = true;
                            }
                            else if (x + i < dicePerRow && diceGrid[x + i, y].dice.GetNumber() == matchNumber)
                            {
                                if (!diceGrid[x + i, y].dice.IsMatched())
                                {
                                    matchedDice.Add(diceGrid[x + i, y].dice);
                                }
                                diceGrid[x + i, y].dice.horMatched = true;
                            }
                        }
                    }
                }

                // Find Vertical Matches
                if (y + matchLength - 1 < dicePerColumn && !dice.vertMatched)
                {
                    bool foundMatch = true;
                    for (int i = 1; i < matchLength; i++)
                    {
                        if (diceGrid[x, y + i].dice.GetNumber() != matchNumber)
                        {
                            foundMatch = false;
                            break;
                        }
                    }

                    if (foundMatch)
                    {
                        Debug.Log("Vertical Match Found: " + matchNumber);
                        for (int i = 0; i < dicePerColumn; i++)
                        {
                            if (i < matchLength)
                            {
                                if (!diceGrid[x, y + i].dice.IsMatched())
                                {
                                    matchedDice.Add(diceGrid[x, y + i].dice);
                                }
                                diceGrid[x, y + i].dice.vertMatched = true;
                            }
                            else if (y + i < dicePerColumn && diceGrid[x, y + i].dice.GetNumber() == matchNumber)
                            {
                                if (!diceGrid[x, y + i].dice.IsMatched())
                                {
                                    matchedDice.Add(diceGrid[x, y + i].dice);
                                }
                                diceGrid[x, y + i].dice.vertMatched = true;
                            }
                        }
                    }
                }

                // Find Diagonal Right Matches
                if (y + matchLength - 1 < dicePerColumn && x + matchLength - 1 < dicePerRow && !dice.diagRMatched)
                {
                    bool foundMatch = true;
                    for (int i = 1; i < matchLength; i++)
                    {
                        if (diceGrid[x + i, y + i].dice.GetNumber() != matchNumber)
                        {
                            foundMatch = false;
                            break;
                        }
                    }

                    if (foundMatch)
                    {
                        Debug.Log("Diagonal Right Match Found: " + matchNumber);
                        for (int i = 0; i < dicePerColumn; i++)
                        {
                            if (i < matchLength)
                            {
                                if (!diceGrid[x + i, y + i].dice.IsMatched())
                                {
                                    matchedDice.Add(diceGrid[x + i, y + i].dice);
                                }
                                diceGrid[x + i, y + i].dice.diagRMatched = true;
                            }
                            else if (y + i < dicePerColumn && x + i < dicePerRow && diceGrid[x + i, y + i].dice.GetNumber() == matchNumber)
                            {
                                if (!diceGrid[x + i, y + i].dice.IsMatched())
                                {
                                    matchedDice.Add(diceGrid[x + i, y + i].dice);
                                }
                                diceGrid[x + i, y + i].dice.diagRMatched = true;
                            }
                        }
                    }
                }

                // Find Diagonal Left Matches
                if (y + matchLength - 1 < dicePerColumn && x - (matchLength - 1) >= 0 && !dice.diagLMatched)
                {
                    bool foundMatch = true;
                    for (int i = 1; i < matchLength; i++)
                    {
                        if (diceGrid[x - i, y + i].dice.GetNumber() != matchNumber)
                        {
                            foundMatch = false;
                            break;
                        }
                    }

                    if (foundMatch)
                    {
                        Debug.Log("Diagonal Left Match Found: " + matchNumber);
                        for (int i = 0; i < dicePerColumn; i++)
                        {
                            if (i < matchLength)
                            {
                                if (!diceGrid[x - i, y + i].dice.IsMatched())
                                {
                                    matchedDice.Add(diceGrid[x - i, y + i].dice);
                                }
                                diceGrid[x - i, y + i].dice.diagLMatched = true;
                            }
                            else if (y + i < dicePerColumn && x - i >= 0 && diceGrid[x - i, y + i].dice.GetNumber() == matchNumber)
                            {
                                if (!diceGrid[x - i, y + i].dice.IsMatched())
                                {
                                    matchedDice.Add(diceGrid[x - i, y + i].dice);
                                }
                                diceGrid[x - i, y + i].dice.diagLMatched = true;
                            }
                        }
                    }
                }
            }
        }

        return matchedDice.ToArray();
    }

    public int GetNumberOfDice(int number)
    {
        int numberOfDice = 0;

        for (int i = 0; i < dicePerRow; i++)
        {
            for (int j = 0; j < dicePerColumn; j++)
            {
                if (diceGrid[i,j].dice.GetNumber() == number)
                {
                    numberOfDice++;
                }
            }
        }

        return numberOfDice;
    }

    Dice[] GetEffectDice(Vector2Int diceIndex, DiceEffect diceEffect)
    {
        List<Dice> effectedDice = new List<Dice>();

        // North effected dice
        for (int i = 1; i < diceEffect.flipsNorth+1; i++)
        {
            int x = diceIndex.x;
            int y = diceIndex.y - i;
            if (y < 0)
            {
                break;
            }
            else
            {
                effectedDice.Add(diceGrid[x, y].dice);
            }
        }

        // North East effected dice
        for (int i = 1; i < diceEffect.flipsNorthEast+1; i++)
        {
            int x = diceIndex.x + i;
            int y = diceIndex.y - i;
            if (y < 0 || x > dicePerRow-1)
            {
                break;
            }
            else
            {
                effectedDice.Add(diceGrid[x, y].dice);
            }
        }

        // East effected dice
        for (int i = 1; i < diceEffect.flipsEast+1; i++)
        {
            int x = diceIndex.x + i;
            int y = diceIndex.y;
            if (x > dicePerRow-1)
            {
                break;
            }
            else
            {
                effectedDice.Add(diceGrid[x, y].dice);
            }
        }

        // South East effected dice
        for (int i = 1; i < diceEffect.flipsSouthEast+1; i++)
        {
            int x = diceIndex.x + i;
            int y = diceIndex.y + i;
            if (y > dicePerColumn-1 || x > dicePerRow-1)
            {
                break;
            }
            else
            {
                effectedDice.Add(diceGrid[x, y].dice);
            }
        }

        // South effected dice
        for (int i = 1; i < diceEffect.flipsSouth+1; i++)
        {
            int x = diceIndex.x;
            int y = diceIndex.y + i;
            if (y > dicePerColumn-1)
            {
                break;
            }
            else
            {
                effectedDice.Add(diceGrid[x, y].dice);
            }
        }

        // South West effected dice
        for (int i = 1; i < diceEffect.flipsSouthWest+1; i++)
        {
            int x = diceIndex.x - i;
            int y = diceIndex.y + i;
            if (y > dicePerColumn-1 || x < 0)
            {
                break;
            }
            else
            {
                effectedDice.Add(diceGrid[x, y].dice);
            }
        }

        // West effected dice
        for (int i = 1; i < diceEffect.flipsWest+1; i++)
        {
            int x = diceIndex.x - i;
            int y = diceIndex.y;
            if (x < 0)
            {
                break;
            }
            else
            {
                effectedDice.Add(diceGrid[x, y].dice);
            }
        }

        // North West effected dice
        for (int i = 1; i < diceEffect.flipsNorthWest+1; i++)
        {
            int x = diceIndex.x - i;
            int y = diceIndex.y - i;
            if (y < 0 || x < 0)
            {
                break;
            }
            else
            {
                effectedDice.Add(diceGrid[x, y].dice);
            }
        }

        return effectedDice.ToArray();
    }
}
