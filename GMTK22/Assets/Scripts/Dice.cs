using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Material highlightMaterial;
    public Material chainMaterial;
    public float diceRollTime;
    public float diceSwitchDelay;
    public float diceRollScale;
    public DiceEffect diceEffect;

    public bool rolling;
    [HideInInspector]
    public bool horMatched;
    [HideInInspector]
    public bool vertMatched;
    [HideInInspector]
    public bool diagRMatched;
    [HideInInspector]
    public bool diagLMatched;
    [HideInInspector]
    public List<Vector2Int> randomNeighbours;

    private int[] diceHistory;
    private int currentDiceHistoryIndex;
    private float currentRollingTime;
    private int currentSpriteFaceIndex;
    private int number;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D diceCollider;
    private Animator animator;
    private Material defaultMaterial;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Awake()
    {
        randomNeighbours = new List<Vector2Int>();
        diceHistory = new int[] { 0, 0, 0, 0, 0, 0 };
        currentDiceHistoryIndex = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        diceCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        audioManager = GetComponent<AudioManager>();
        defaultMaterial = spriteRenderer.material;
        currentSpriteFaceIndex = 0;
        number = Random.Range(1, 7);
        rolling = false;
        horMatched = false;
        vertMatched = false;
        diagRMatched = false;
        diagLMatched = false;
        RenderDiceFace();
        //RollDice();
    }

    public void ResetDiceHistory()
    {
        diceHistory = new int[] { 0, 0, 0, 0, 0, 0 };
        currentDiceHistoryIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        RenderDiceFace();
        if (number != currentSpriteFaceIndex + 1 || spriteRenderer.sprite != diceEffect.diceFaces[number-1])
        {
            //RenderDiceFace();
        }
    }

    void RenderDiceFace()
    {
        currentSpriteFaceIndex = number-1;
        spriteRenderer.sprite = diceEffect.diceFaces[currentSpriteFaceIndex];
        //spriteRenderer.material.color = diceEffect.tint;
    }

    public void RollDice()
    {
        if (!rolling)
        {
            diceHistory[currentDiceHistoryIndex] = number;
            currentDiceHistoryIndex++;
            if (currentDiceHistoryIndex >= diceHistory.Length)
            {
                ResetDiceHistory();
            }

            StartCoroutine(RunDiceRollingAnimation());
            StartCoroutine(RunDiceScalingAnimation());
            //Debug.Log("Roll Dice: " + number);
            randomNeighbours = new List<Vector2Int>();
        }
    }

    public void PlayDiceRollSound()
    {
        audioManager.Play("Roll");
    }

    public void MatchDice(bool playSound = false)
    {
        StartCoroutine(PlayMatchAnimtion(playSound));
    }

    IEnumerator PlayMatchAnimtion(bool playSound)
    {
        animator.SetTrigger("Match");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        RollDice();
        ResetMatched();
        if (playSound)
        {
            audioManager.Play("Roll");
        }
    }

    public void ResetMatched()
    {
        horMatched = false;
        vertMatched = false;
        diagRMatched = false;
        diagLMatched = false;
    }

    public bool IsMatched()
    {
        return horMatched || vertMatched || diagRMatched || diagLMatched;
    }

    IEnumerator RunDiceRollingAnimation()
    {
        rolling = true;
        currentRollingTime = 0.0f;
        while (currentRollingTime < diceRollTime)
        {
            int newNumber = number;
            while (newNumber == number)
            {
                newNumber = Random.Range(1, 7);
            }
            number = newNumber;
            yield return new WaitForSeconds(diceSwitchDelay);
            currentRollingTime += diceSwitchDelay;
        }

        int preNumber = Random.Range(1, 7);
        while (ArrayContains(diceHistory, preNumber))
        {
            preNumber = Random.Range(1, 7);
        }

        number = preNumber;

        rolling = false;
    }

    bool ArrayContains(int[] array, int num)
    {
        foreach (int number in array)
        {
            if (number == num)
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator RunDiceScalingAnimation()
    {
        while (currentRollingTime < diceRollTime)
        {
            if (currentRollingTime < diceRollTime * 0.5f)
            {
                float targetScale = 1.0f + (diceRollScale - 1.0f) * (currentRollingTime) / (diceRollTime * 0.5f);
                gameObject.transform.localScale = new Vector3(targetScale, targetScale, 1.0f);
                yield return new WaitForSeconds(0.01f);
            }
            else
            {
                float targetScale = 1.0f + (diceRollScale - 1.0f) * (1 - (currentRollingTime - diceRollTime * 0.5f) / (diceRollTime * 0.5f));
                gameObject.transform.localScale = new Vector3(targetScale, targetScale, 1.0f);
                yield return new WaitForSeconds(0.01f);
            }
        }
        gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public bool IsPointOnDice(Vector2 position)
    {
        return diceCollider.bounds.Contains(position);
    }

    public void HighlightDice(bool chainReaction = false)
    {
        if (!chainReaction)
        {
            spriteRenderer.material = highlightMaterial;
        }
        else
        {
            spriteRenderer.material = chainMaterial;
        }
    }

    public void UnhighlightDice()
    {
        spriteRenderer.material = defaultMaterial;
    }

    public int GetNumber()
    {
        return number;
    }
}
