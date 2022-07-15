using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;

public class DiceTableUI : MonoBehaviour
{
    public DiceTable diceTable;

    public GameObject diceCount;
    public GameObject dice1Score;
    public GameObject dice2Score;
    public GameObject dice3Score;
    public GameObject dice4Score;
    public GameObject dice5Score;
    public GameObject dice6Score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int num1Dice = diceTable.GetNumberOfDice(1);
        int num2Dice = diceTable.GetNumberOfDice(2);
        int num3Dice = diceTable.GetNumberOfDice(3);
        int num4Dice = diceTable.GetNumberOfDice(4);
        int num5Dice = diceTable.GetNumberOfDice(5);
        int num6Dice = diceTable.GetNumberOfDice(6);

        dice1Score.GetComponent<TMPro.TextMeshProUGUI>().text = num1Dice.ToString();
        dice2Score.GetComponent<TMPro.TextMeshProUGUI>().text = num2Dice.ToString();
        dice3Score.GetComponent<TMPro.TextMeshProUGUI>().text = num3Dice.ToString();
        dice4Score.GetComponent<TMPro.TextMeshProUGUI>().text = num4Dice.ToString();
        dice5Score.GetComponent<TMPro.TextMeshProUGUI>().text = num5Dice.ToString();
        dice6Score.GetComponent<TMPro.TextMeshProUGUI>().text = num6Dice.ToString();

        int totalDiceCount = 1 * num1Dice + 2 * num2Dice + 3 * num3Dice + 4 * num4Dice + 5 * num5Dice + 6 * num6Dice;
        diceCount.GetComponent<TMPro.TextMeshProUGUI>().text = totalDiceCount.ToString();
    }
}
