using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RingDiceEffect", menuName = "Dice Effects/Ring")]
public class RingDiceEffect : DiceEffect
{
    public override (List<Vector2Int>, EffectCaveat) GetEffectIndices(Vector2Int diceIndex)
    {
        List<Vector2Int> diceIndices = new List<Vector2Int>();

        diceIndices.Add(diceIndex + new Vector2Int(0, 2));
        diceIndices.Add(diceIndex + new Vector2Int(0, -2));
        diceIndices.Add(diceIndex + new Vector2Int(1, 2));
        diceIndices.Add(diceIndex + new Vector2Int(1, -2));
        diceIndices.Add(diceIndex + new Vector2Int(-1, -2));
        diceIndices.Add(diceIndex + new Vector2Int(-1, 2));
        diceIndices.Add(diceIndex + new Vector2Int(2, 2));
        diceIndices.Add(diceIndex + new Vector2Int(2, 1));
        diceIndices.Add(diceIndex + new Vector2Int(2, 0));
        diceIndices.Add(diceIndex + new Vector2Int(2, -1));
        diceIndices.Add(diceIndex + new Vector2Int(2, -2));
        diceIndices.Add(diceIndex + new Vector2Int(-2, -2));
        diceIndices.Add(diceIndex + new Vector2Int(-2, 2));
        diceIndices.Add(diceIndex + new Vector2Int(-2, -1));
        diceIndices.Add(diceIndex + new Vector2Int(-2, 1));
        diceIndices.Add(diceIndex + new Vector2Int(-2, 0));

        return (diceIndices, effectCaveat);
    }
}
