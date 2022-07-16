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
    public float affectedDiceRollDelay;

    private DiceSlot[,] diceGrid;
    private float gridHeight;
    private float gridWidth;
    private float diceWidth;
    private float diceHeight;
    private Dice[] affectedDice;
    private int[] affectedNumbers;
    private DiceEffect[] diceEffectsByNumber;
    private DiceEffectPool diceEffectPool;

    // Start is called before the first frame update
    void Start()
    {
        diceGrid = new DiceSlot[dicePerRow, dicePerColumn];
        affectedNumbers = new int[] { 0, 0, 0, 0, 0, 0 };
        diceEffectPool = FindObjectOfType<DiceEffectPool>();
        diceEffectsByNumber = new DiceEffect[6];

        CreateDice();

        for (int i = 0; i < dicePerRow; i++)
        {
            for (int j = 0; j < dicePerColumn; j++)
            {
                Dice dice = diceGrid[i, j].dice;
                diceEffectsByNumber[dice.GetNumber() - 1] = dice.diceEffect;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!AreAnyDiceRolling())
        {
            if (!AreAnyDiceMatched())
            {
                DiceEffect[] newDiceEffects = new DiceEffect[6];
                foreach (int number in affectedNumbers)
                {
                    if (number > 0 && newDiceEffects[number - 1] == null)
                    {
                        newDiceEffects[number-1] = diceEffectPool.GetRandomDiceEffect();
                    }
                }

                for (int i = 0; i < dicePerRow; i++)
                {
                    for (int j = 0; j < dicePerColumn; j++)
                    {
                        Dice dice = diceGrid[i, j].dice;
                        if (dice.GetNumber() == affectedNumbers[dice.GetNumber() - 1])
                        {
                            dice.diceEffect = newDiceEffects[dice.GetNumber() - 1];
                            diceEffectsByNumber[dice.GetNumber() - 1] = newDiceEffects[dice.GetNumber() - 1];
                        }

                        if (dice.diceEffect != diceEffectsByNumber[dice.GetNumber() - 1])
                        {
                            dice.diceEffect = diceEffectsByNumber[dice.GetNumber() - 1];
                        }
                    }
                }

                affectedNumbers = new int[] { 0, 0, 0, 0, 0, 0 };

                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0.0f;

                Dice selectedDice = null;
                Vector2Int selectedDiceIndex = new Vector2Int(0, 0);
                for (int i = 0; i < dicePerRow; i++)
                {
                    for (int j = 0; j < dicePerColumn; j++)
                    {
                        Dice dice = diceGrid[i, j].dice;
                        if (dice.IsPointOnDice(mouseWorldPosition))
                        {
                            dice.HighlightDice();
                            selectedDice = diceGrid[i, j].dice;
                            selectedDiceIndex = new Vector2Int(i, j);
                        }
                        else
                        {
                            dice.UnhighlightDice();
                        }
                    }
                }

                affectedDice = new Dice[0];
                if (selectedDice)
                {
                    affectedDice = GetAffectedDice(selectedDice, selectedDiceIndex);
                    foreach (Dice dice in affectedDice)
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

                        StartCoroutine(RollaffectedDice());
                    }
                }
            }

            if (!AreAnyDiceRolling())
            {
                Dice[] matchedDice = DetectMatches();

                foreach (Dice matchedDie in matchedDice)
                {
                    affectedNumbers[matchedDie.GetNumber() - 1] = matchedDie.GetNumber();
                    matchedDie.MatchDice();
                }
            }
        }
    }

    IEnumerator RollaffectedDice()
    {
        yield return new WaitForSeconds(affectedDiceRollDelay);
        foreach (Dice dice in affectedDice)
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
                Vector2 dicePos = topLeftPos + new Vector2(i * diceSeparation.x, j * diceSeparation.y);
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
                if (diceGrid[x, y].dice.rolling)
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
                        if (diceGrid[x + i, y].dice.GetNumber() != matchNumber)
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
                            else
                            {
                                break;
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
                            else
                            {
                                break;
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
                            else
                            {
                                break;
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
                            else
                            {
                                break;
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
                if (diceGrid[i, j].dice.GetNumber() == number)
                {
                    numberOfDice++;
                }
            }
        }

        return numberOfDice;
    }

    Dice[] GetAffectedDice(Dice dice, Vector2Int diceIndex)
    {
        List<Dice> affectedDice = new List<Dice>();

        List<Vector2Int> affectedIndices = dice.diceEffect.GetEffectIndices(diceIndex);

        foreach (Vector2Int affectedIndex in affectedIndices)
        {
            if (affectedIndex.x >= 0 && affectedIndex.x < dicePerRow && affectedIndex.y >= 0 && affectedIndex.y < dicePerColumn)
            {
                affectedDice.Add(diceGrid[affectedIndex.x, affectedIndex.y].dice);
            }
        }

        return affectedDice.ToArray();
    }

    public DiceEffect GetDiceEffectByNumber(int number)
    {
        return diceEffectsByNumber[number - 1];
    }
}
