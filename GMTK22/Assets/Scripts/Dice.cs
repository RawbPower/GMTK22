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
    public DiceEffect diceEffect1;
    public DiceEffect diceEffect2;
    public DiceEffect diceEffect3;
    public DiceEffect diceEffect4;
    public DiceEffect diceEffect5;
    public DiceEffect diceEffect6;

    private int currentSpriteFaceIndex;
    private int number;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D diceCollider;
    private Material defaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        diceCollider = GetComponent<BoxCollider2D>();
        defaultMaterial = spriteRenderer.material;
        currentSpriteFaceIndex = 0;
        number = 0;
        RollDice();
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
        number = Random.Range(1, 7);
        Debug.Log("Roll Dice: " + number);
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
