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

    public Vector2 offset;
    public int dicePerRow;
    public int dicePerColumn;
    public float diceSpacing;
    public int matchLength = 3;
    public GameObject dicePrefab;
    public float affectedDiceRollDelay;
    public Timer timer;
    public GameObject endGameOverlay;
    public GameObject scoreUI;
    public DiceEffect defaultDiceEffect;
    public float comboIncreasePerMatch;

    private bool chainReaction;
    private int score;
    private float combo;
    private DiceSlot[,] diceGrid;
    private float gridHeight;
    private float gridWidth;
    private float diceWidth;
    private float diceHeight;
    private List<Dice> affectedDice;
    private int[] affectedNumbers;
    private DiceEffect[] diceEffectsByNumber;
    private DiceEffectPool diceEffectPool;
    private bool gameOver;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        chainReaction = false;
        diceGrid = new DiceSlot[dicePerRow, dicePerColumn];
        affectedNumbers = new int[] { 0, 0, 0, 0, 0, 0 };
        diceEffectPool = FindObjectOfType<DiceEffectPool>();
        diceEffectsByNumber = new DiceEffect[6] { defaultDiceEffect, defaultDiceEffect, defaultDiceEffect, defaultDiceEffect, defaultDiceEffect, defaultDiceEffect };
        gameOver = false;
        endGameOverlay.SetActive(false);
        audioManager = GetComponent<AudioManager>();
        affectedDice = new List<Dice>();

        CreateDice();

        Dice[] matchedDice = DetectMatches(false);
        while (matchedDice.Length > 0)
        {
            ClearDice();
            diceGrid = new DiceSlot[dicePerRow, dicePerColumn];
            CreateDice();
            matchedDice = DetectMatches(false);
        }

        score = 0;
        combo = 1.0f;
        scoreUI.GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + score.ToString();

        for (int i = 0; i < dicePerRow; i++)
        {
            for (int j = 0; j < dicePerColumn; j++)
            {
                Dice dice = diceGrid[i, j].dice;
                diceEffectsByNumber[dice.GetNumber() - 1] = dice.diceEffect;
            }
        }

        if (timer)
        {
            timer.StartTimer();
        }

        audioManager.Play("Start");
    }

    private void OnEnable()
    {
        Timer.OnTimerEnd += GameOver;
    }

    private void OnDisable()
    {
        Timer.OnTimerEnd -= GameOver;
    }

    void GameOver()
    {
        gameOver = true;
        endGameOverlay.SetActive(true);
        endGameOverlay.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Score: " + score.ToString();
    }

    public void SetChainReaction(bool chain)
    {
        chainReaction = chain;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            for (int i = 0; i < diceEffectsByNumber.Length; i++)
            {
                if (diceEffectsByNumber[i] == null)
                {
                    bool foundDice = false;
                    for (int x = 0; x < dicePerRow; x++)
                    {
                        for (int y = 0; y < dicePerColumn; y++)
                        {
                            Dice dice = diceGrid[x, y].dice;
                            if (dice.GetNumber() - 1 == i)
                            {
                                diceEffectsByNumber[dice.GetNumber() - 1] = dice.diceEffect;
                                foundDice = true;
                            }
                        }
                    }

                    if (!foundDice)
                    {
                        diceEffectsByNumber[i] = defaultDiceEffect;
                    }
                }
            }

            DiceEffect[] newDiceEffects = new DiceEffect[6];
            foreach (int number in affectedNumbers)
            {
                if (number > 0 && newDiceEffects[number - 1] == null)
                {
                    bool uniqueDiceEffect = false;
                    DiceEffect randomDiceEffect = null;
                    int attempts = 0;
                    while (!uniqueDiceEffect && attempts < 20)
                    {
                        randomDiceEffect = diceEffectPool.GetRandomDiceEffect(score);
                        uniqueDiceEffect = IsValidRandomDiceEffect(randomDiceEffect);
                        attempts++;
                    }

                    if (attempts >= 20)
                    {
                        Debug.Log("Almost Crashed!");
                    }
                    newDiceEffects[number - 1] = randomDiceEffect;
                    diceEffectsByNumber[number - 1] = randomDiceEffect;
                }
            }

            for (int i = 0; i < dicePerRow; i++)
            {
                for (int j = 0; j < dicePerColumn; j++)
                {
                    Dice dice = diceGrid[i, j].dice;
                    if (dice.GetNumber() == affectedNumbers[dice.GetNumber() - 1])
                    {
                        dice.diceEffect = diceEffectsByNumber[dice.GetNumber() - 1];
                        //diceEffectsByNumber[dice.GetNumber() - 1] = newDiceEffects[dice.GetNumber() - 1];
                    }

                    if (dice.diceEffect != diceEffectsByNumber[dice.GetNumber() - 1] && !dice.rolling)
                    {
                        dice.diceEffect = diceEffectsByNumber[dice.GetNumber() - 1];
                    }
                }
            }

            affectedNumbers = new int[] { 0, 0, 0, 0, 0, 0 };

            if (!AreAnyDiceRolling())
            {
                if (!AreAnyDiceMatched())
                {
                    /*DiceEffect[] newDiceEffects = new DiceEffect[6];
                    foreach (int number in affectedNumbers)
                    {
                        if (number > 0 && newDiceEffects[number - 1] == null)
                        {
                            bool uniqueDiceEffect = false;
                            DiceEffect randomDiceEffect = null;
                            int attempts = 0;
                            while (!uniqueDiceEffect && attempts < 20)
                            {
                                randomDiceEffect = diceEffectPool.GetRandomDiceEffect();
                                uniqueDiceEffect = IsValidRandomDiceEffect(randomDiceEffect);
                                attempts++;
                            }

                            if (attempts >= 20)
                            {
                                Debug.Log("Almost Crashed!");
                            }
                            newDiceEffects[number-1] = randomDiceEffect;
                            diceEffectsByNumber[number - 1] = randomDiceEffect;
                        }
                    }

                    for (int i = 0; i < dicePerRow; i++)
                    {
                        for (int j = 0; j < dicePerColumn; j++)
                        {
                            Dice dice = diceGrid[i, j].dice;
                            if (dice.GetNumber() == affectedNumbers[dice.GetNumber() - 1])
                            {
                                dice.diceEffect = diceEffectsByNumber[dice.GetNumber() - 1];
                                //diceEffectsByNumber[dice.GetNumber() - 1] = newDiceEffects[dice.GetNumber() - 1];
                            }

                            if (dice.diceEffect != diceEffectsByNumber[dice.GetNumber() - 1])
                            {
                                dice.diceEffect = diceEffectsByNumber[dice.GetNumber() - 1];
                            }
                        }
                    }

                    affectedNumbers = new int[] { 0, 0, 0, 0, 0, 0 };*/

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
                                dice.HighlightDice(chainReaction);
                                selectedDice = diceGrid[i, j].dice;
                                selectedDiceIndex = new Vector2Int(i, j);
                            }
                            else
                            {
                                dice.UnhighlightDice();
                            }
                        }
                    }

                    affectedDice = new List<Dice>();
                    if (selectedDice)
                    {
                        GetAffectedDice(selectedDice, selectedDiceIndex, ref affectedDice);
                        foreach (Dice dice in affectedDice)
                        {
                            dice.HighlightDice(chainReaction);
                        }
                    }

                    // If dice is clicked roll it
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (selectedDice)
                        {
                            combo = 1.0f;
                            selectedDice.RollDice();
                            audioManager.Play("Roll");

                            StartCoroutine(RollAffectedDice());
                        }
                    }
                }

                if (!AreAnyDiceRolling())
                {
                    Dice[] matchedDice = DetectMatches();
                    bool playSound = true;

                    foreach (Dice matchedDie in matchedDice)
                    {
                        affectedNumbers[matchedDie.GetNumber() - 1] = matchedDie.GetNumber();
                        matchedDie.MatchDice(playSound);
                        playSound = false;
                    }
                }
            }
        }
    }

    bool IsValidRandomDiceEffect(DiceEffect randomDiceEffect)
    {
        if (randomDiceEffect is NormalDiceEffect)
        {
            return true;
        }

        bool uniqueDiceEffect = true;
        foreach (DiceEffect effect in diceEffectsByNumber)
        {
            if (effect == randomDiceEffect)
            {
                uniqueDiceEffect = false;
            }
        }

        return uniqueDiceEffect;
    }

    IEnumerator RollAffectedDice()
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

        Vector2 topLeftPos = new Vector2(-gridWidth * 0.5f + diceWidth * 0.5f + offset.x, gridHeight * 0.5f - diceHeight * 0.5f + offset.y);
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

    void ClearDice()
    {
        for (int i = 0; i < dicePerRow; i++)
        {
            for (int j = 0; j < dicePerColumn; j++)
            {
                Destroy(diceGrid[i, j].dice.gameObject);
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

    private Dice[] DetectMatches(bool playSounds = true)
    {
        int matchesFound = 0;
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
                        matchesFound++;
                        int baseScore = 300;
                        bool fourMatch = false;
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
                                baseScore = 500;
                                fourMatch = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        score += Mathf.FloorToInt(combo * (baseScore * Mathf.FloorToInt(1 + (matchesFound - 1) * 0.5f)));

                        if (playSounds)
                        {
                            if (fourMatch)
                            {
                                audioManager.Play("Match4");
                            }
                            else
                            {
                                audioManager.Play("Match3");
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
                        matchesFound++;
                        int baseScore = 300;
                        bool fourMatch = false;
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
                                baseScore = 500;
                                fourMatch = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        score += Mathf.FloorToInt(combo * (baseScore * Mathf.FloorToInt(1 + (matchesFound - 1) * 0.5f)));

                        if (playSounds)
                        {
                            if (fourMatch)
                            {
                                audioManager.Play("Match4");
                            }
                            else
                            {
                                audioManager.Play("Match3");
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
                        matchesFound++;
                        int baseScore = 300;
                        bool fourMatch = false;
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
                                baseScore = 500;
                                fourMatch = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        score += Mathf.FloorToInt(combo * (baseScore * Mathf.FloorToInt(1 + (matchesFound - 1) * 0.5f)));

                        if (playSounds)
                        {
                            if (fourMatch)
                            {
                                audioManager.Play("Match4");
                            }
                            else
                            {
                                audioManager.Play("Match3");
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
                        matchesFound++;
                        int baseScore = 300;
                        bool fourMatch = false;
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
                                baseScore = 500;
                                fourMatch = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        score += Mathf.FloorToInt(combo * (baseScore * Mathf.FloorToInt(1 + (matchesFound - 1) * 0.5f)));

                        if (playSounds)
                        {
                            if (fourMatch)
                            {
                                audioManager.Play("Match4");
                            }
                            else
                            {
                                audioManager.Play("Match3");
                            }
                        }
                    }
                }
            }
        }

        if (matchesFound > 0)
        {
            scoreUI.GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + score.ToString();
            combo += comboIncreasePerMatch;
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

    void GetAffectedDice(Dice dice, Vector2Int diceIndex, ref List<Dice> affectedDice)
    {
        if (!affectedDice.Contains(dice))
        {
            affectedDice.Add(dice);
        }

        (List<Vector2Int>, DiceEffect.EffectCaveat) affectedIndicesTuple = dice.diceEffect.GetEffectIndices(diceIndex);
        List<Vector2Int> affectedIndices = affectedIndicesTuple.Item1;
        DiceEffect.EffectCaveat caveat = affectedIndicesTuple.Item2;

        // Affecting itself means affecting all dice
        if (affectedIndices.Count == 1 && affectedIndices[0] == diceIndex)
        {
            for (int x = 0; x < dicePerRow; x++)
            {
                for (int y = 0; y < dicePerColumn; y++)
                {
                    if (caveat == DiceEffect.EffectCaveat.NONE || caveat == DiceEffect.EffectCaveat.LOCK || caveat == DiceEffect.EffectCaveat.RANDOM_DIRECTION)
                    {
                        if (chainReaction)
                        {
                            if (!affectedDice.Contains(diceGrid[x, y].dice))
                            {
                                GetAffectedDice(diceGrid[x, y].dice, new Vector2Int(x, y), ref affectedDice);
                            }
                            affectedDice.Add(diceGrid[x, y].dice);
                        }
                        else
                        {
                            affectedDice.Add(diceGrid[x, y].dice);
                        }
                    }
                    else if (caveat == DiceEffect.EffectCaveat.ODD_EVEN)
                    {
                        if (diceGrid[x, y].dice.GetNumber() % 2 == dice.GetNumber() % 2)
                        {
                            if (chainReaction)
                            {
                                if (!affectedDice.Contains(diceGrid[x, y].dice))
                                {
                                    GetAffectedDice(diceGrid[x, y].dice, new Vector2Int(x, y), ref affectedDice);
                                }
                                affectedDice.Add(diceGrid[x, y].dice);
                            }
                            else
                            {
                                affectedDice.Add(diceGrid[x, y].dice);
                            }
                        }
                    }
                    else if (caveat == DiceEffect.EffectCaveat.SAME_FACE)
                    {
                        if (diceGrid[x, y].dice.GetNumber() == dice.GetNumber())
                        {
                            if (chainReaction)
                            {
                                if (!affectedDice.Contains(diceGrid[x, y].dice))
                                {
                                    GetAffectedDice(diceGrid[x, y].dice, new Vector2Int(x, y), ref affectedDice);
                                }
                                affectedDice.Add(diceGrid[x, y].dice);
                            }
                            else
                            {
                                affectedDice.Add(diceGrid[x, y].dice);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (caveat == DiceEffect.EffectCaveat.RANDOM_DIRECTION)
            {
                List<Vector2Int> newAffectedIndices = new List<Vector2Int>();
                if (dice.randomNeighbours.Count == 0)
                {
                    int distance = Mathf.Max(1, dice.diceEffect.distance);
                    int randomIndex = Random.Range(0, (affectedIndices.Count / distance));
                    for (int i = randomIndex; i < randomIndex + distance; i++)
                    {
                        if (affectedIndices[i].x >= 0 && affectedIndices[i].x < dicePerRow && affectedIndices[i].y >= 0 && affectedIndices[i].y < dicePerColumn && diceGrid[affectedIndices[i].x, affectedIndices[i].y].dice.diceEffect.effectCaveat != DiceEffect.EffectCaveat.LOCK)
                        {
                            newAffectedIndices.Add(affectedIndices[i]);
                            dice.randomNeighbours.Add(affectedIndices[i]);
                        }
                    }
                }
                else
                {
                    foreach (Vector2Int randomNeighbour in dice.randomNeighbours)
                    {
                        newAffectedIndices.Add(randomNeighbour);
                    }
                }

                affectedIndices = newAffectedIndices;
            }

            {
                foreach (Vector2Int affectedIndex in affectedIndices)
                {
                    if (affectedIndex.x >= 0 && affectedIndex.x < dicePerRow && affectedIndex.y >= 0 && affectedIndex.y < dicePerColumn && diceGrid[affectedIndex.x, affectedIndex.y].dice.diceEffect.effectCaveat != DiceEffect.EffectCaveat.LOCK)
                    {
                        if (caveat == DiceEffect.EffectCaveat.NONE || caveat == DiceEffect.EffectCaveat.LOCK || caveat == DiceEffect.EffectCaveat.RANDOM_DIRECTION)
                        {
                            if (chainReaction)
                            {
                                if (!affectedDice.Contains(diceGrid[affectedIndex.x, affectedIndex.y].dice))
                                {
                                    GetAffectedDice(diceGrid[affectedIndex.x, affectedIndex.y].dice, affectedIndex, ref affectedDice);
                                }
                                affectedDice.Add(diceGrid[affectedIndex.x, affectedIndex.y].dice);
                            }
                            else
                            {
                                affectedDice.Add(diceGrid[affectedIndex.x, affectedIndex.y].dice);
                            }
                        }
                        else if (caveat == DiceEffect.EffectCaveat.ODD_EVEN)
                        {
                            if (diceGrid[affectedIndex.x, affectedIndex.y].dice.GetNumber() % 2 == dice.GetNumber() % 2)
                            {
                                if (chainReaction)
                                {
                                    if (!affectedDice.Contains(diceGrid[affectedIndex.x, affectedIndex.y].dice))
                                    {
                                        GetAffectedDice(diceGrid[affectedIndex.x, affectedIndex.y].dice, affectedIndex, ref affectedDice);
                                    }
                                    affectedDice.Add(diceGrid[affectedIndex.x, affectedIndex.y].dice);
                                }
                                else
                                {
                                    affectedDice.Add(diceGrid[affectedIndex.x, affectedIndex.y].dice);
                                }
                            }
                        }
                        else if (caveat == DiceEffect.EffectCaveat.SAME_FACE)
                        {
                            if (diceGrid[affectedIndex.x, affectedIndex.y].dice.GetNumber() == dice.GetNumber())
                            {
                                if (chainReaction)
                                {
                                    if (!affectedDice.Contains(diceGrid[affectedIndex.x, affectedIndex.y].dice))
                                    {
                                        GetAffectedDice(diceGrid[affectedIndex.x, affectedIndex.y].dice, affectedIndex, ref affectedDice);
                                    }
                                    affectedDice.Add(diceGrid[affectedIndex.x, affectedIndex.y].dice);
                                }
                                else
                                {
                                    affectedDice.Add(diceGrid[affectedIndex.x, affectedIndex.y].dice);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public DiceEffect GetDiceEffectByNumber(int number)
    {
        return diceEffectsByNumber[number - 1];
    }
}
