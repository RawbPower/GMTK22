using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Image dice1Image;
    public Image dice2Image;
    public Image dice3Image;
    public Image dice4Image;
    public Image dice5Image;
    public Image dice6Image;
    public Image chain1Image;
    public Image chain2Image;
    public Image chain3Image;
    public Image chain4Image;
    public Image chain5Image;
    public Image chain6Image;
    public GameObject chainReactionUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!diceTable.AreAnyDiceRolling())
        {
            int num1Dice = diceTable.GetNumberOfDice(1);
            int num2Dice = diceTable.GetNumberOfDice(2);
            int num3Dice = diceTable.GetNumberOfDice(3);
            int num4Dice = diceTable.GetNumberOfDice(4);
            int num5Dice = diceTable.GetNumberOfDice(5);
            int num6Dice = diceTable.GetNumberOfDice(6);

            dice1Score.GetComponent<TMPro.TextMeshProUGUI>().text = diceTable.GetDiceEffectByNumber(1).GetName();
            dice2Score.GetComponent<TMPro.TextMeshProUGUI>().text = diceTable.GetDiceEffectByNumber(2).GetName();
            dice3Score.GetComponent<TMPro.TextMeshProUGUI>().text = diceTable.GetDiceEffectByNumber(3).GetName();
            dice4Score.GetComponent<TMPro.TextMeshProUGUI>().text = diceTable.GetDiceEffectByNumber(4).GetName();
            dice5Score.GetComponent<TMPro.TextMeshProUGUI>().text = diceTable.GetDiceEffectByNumber(5).GetName();
            dice6Score.GetComponent<TMPro.TextMeshProUGUI>().text = diceTable.GetDiceEffectByNumber(6).GetName();

            dice1Image.sprite = diceTable.GetDiceEffectByNumber(1).diceFaces[0];
            dice2Image.sprite = diceTable.GetDiceEffectByNumber(2).diceFaces[1];
            dice3Image.sprite = diceTable.GetDiceEffectByNumber(3).diceFaces[2];
            dice4Image.sprite = diceTable.GetDiceEffectByNumber(4).diceFaces[3];
            dice5Image.sprite = diceTable.GetDiceEffectByNumber(5).diceFaces[4];
            dice6Image.sprite = diceTable.GetDiceEffectByNumber(6).diceFaces[5];

            chain1Image.enabled = diceTable.GetIsChainReactive(1);
            chain2Image.enabled = diceTable.GetIsChainReactive(2);
            chain3Image.enabled = diceTable.GetIsChainReactive(3);
            chain4Image.enabled = diceTable.GetIsChainReactive(4);
            chain5Image.enabled = diceTable.GetIsChainReactive(5);
            chain6Image.enabled = diceTable.GetIsChainReactive(6);

            chainReactionUI.SetActive(diceTable.GetIsChainReaction());

            int totalDiceCount = 1 * num1Dice + 2 * num2Dice + 3 * num3Dice + 4 * num4Dice + 5 * num5Dice + 6 * num6Dice;
            diceCount.GetComponent<TMPro.TextMeshProUGUI>().text = totalDiceCount.ToString();
        }
    }

    public void SetChainReaction(bool chainReaction)
    {
        diceTable.SetChainReaction(chainReaction);
    }
}
