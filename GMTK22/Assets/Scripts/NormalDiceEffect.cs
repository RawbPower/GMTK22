using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalDiceEffect", menuName = "Dice Effects/Normal")]
public class NormalDiceEffect : DiceEffect
{
    public override List<Vector2Int> GetEffectIndices(Vector2Int diceIndex)
    {
        List<Vector2Int> diceIndices = new List<Vector2Int>();

        return diceIndices;
    }
}
