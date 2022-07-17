using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllDiceEffect", menuName = "Dice Effects/All")]
public class AllDiceEffect : DiceEffect
{
    public override (List<Vector2Int>, EffectCaveat) GetEffectIndices(Vector2Int diceIndex)
    {
        List<Vector2Int> diceIndices = new List<Vector2Int>();

        diceIndices.Add(diceIndex);

        return (diceIndices, effectCaveat);
    }
}
