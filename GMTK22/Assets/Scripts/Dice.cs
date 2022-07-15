using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Sprite[] diceFaces;

    private int currentSpriteFaceIndex;
    private int number;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D diceCollider;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        diceCollider = GetComponent<BoxCollider2D>();
        currentSpriteFaceIndex = 0;
        number = 0;
        RollDice();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0.0f;
            if (diceCollider.bounds.Contains(mouseWorldPosition))
            {
                RollDice();
                Debug.Log("Inside Dice: " + gameObject.name);
            }
        }

        if (number != currentSpriteFaceIndex)
        {
            RenderDiceFace();
        }
    }

    void RenderDiceFace()
    {
        currentSpriteFaceIndex = number;
        spriteRenderer.sprite = diceFaces[currentSpriteFaceIndex];
    }

    void RollDice()
    {
        number = Random.Range(0, 6);
    }
}
