using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DiceEffect
{
    public int flipsNorth;
    public int flipsNorthEast;
    public int flipsEast;
    public int flipsSouthEast;
    public int flipsSouth;
    public int flipsSouthWest;
    public int flipsWest;
    public int flipsNorthWest;
}

public class Dice : MonoBehaviour
{
    public Sprite[] diceFaces;
    public Material highlightMaterial;
    public float diceRollTime;
    public float diceSwitchDelay;
    public float diceRollScale;
    public DiceEffect diceEffect1;
    public DiceEffect diceEffect2;
    public DiceEffect diceEffect3;
    public DiceEffect diceEffect4;
    public DiceEffect diceEffect5;
    public DiceEffect diceEffect6;

    public bool rolling;
    [HideInInspector]
    public bool horMatched;
    [HideInInspector]
    public bool vertMatched;
    [HideInInspector]
    public bool diagRMatched;
    [HideInInspector]
    public bool diagLMatched;

    private float currentRollingTime;
    private int currentSpriteFaceIndex;
    private int number;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D diceCollider;
    private Animator animator;
    private Material defaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        diceCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        defaultMaterial = spriteRenderer.material;
        currentSpriteFaceIndex = 0;
        number = Random.Range(1, 7);
        rolling = false;
        horMatched = false;
        vertMatched = false;
        diagRMatched = false;
        diagLMatched = false;
        //RollDice();
    }

    // Update is called once per frame
    void Update()
    {
        if (number-1 != currentSpriteFaceIndex)
        {
            RenderDiceFace();
        }
    }

    void RenderDiceFace()
    {
        currentSpriteFaceIndex = number-1;
        spriteRenderer.sprite = diceFaces[currentSpriteFaceIndex];
    }

    public void RollDice()
    {
        if (!rolling)
        {
            StartCoroutine(RunDiceRollingAnimation());
            //StartCoroutine(RunDiceScalingAnimation());
            Debug.Log("Roll Dice: " + number);
        }
    }

    public void MatchDice()
    {
        StartCoroutine(PlayMatchAnimtion());
    }

    IEnumerator PlayMatchAnimtion()
    {
        animator.SetTrigger("Match");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        RollDice();
        ResetMatched();
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
        rolling = false;
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

    public void HighlightDice()
    {
        spriteRenderer.material = highlightMaterial;
    }

    public void UnhighlightDice()
    {
        spriteRenderer.material = defaultMaterial;
    }

    public int GetNumber()
    {
        return number;
    }

    public DiceEffect GetDiceEffect()
    {
        switch (number)
        {
            case 1:
                return diceEffect1;
            case 2:
                return diceEffect2;
            case 3:
                return diceEffect3;
            case 4:
                return diceEffect4;
            case 5:
                return diceEffect5;
            case 6:
                return diceEffect6;
        }

        DiceEffect nullDiceEffect = new DiceEffect();
        return nullDiceEffect;
    }
}
