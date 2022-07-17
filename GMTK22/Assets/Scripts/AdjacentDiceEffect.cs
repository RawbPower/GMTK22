using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdjacentDiceEffect", menuName = "Dice Effects/Adjacent")]
public class AdjacentDiceEffect : DiceEffect
{
    public int adjacentNorth;
    public int adjacentNorthEast;
    public int adjacentEast;
    public int adjacentSouthEast;
    public int adjacentSouth;
    public int adjacentSouthWest;
    public int adjacentWest;
    public int adjacentNorthWest;

    public override (List<Vector2Int>, EffectCaveat) GetEffectIndices(Vector2Int diceIndex)
    {
        List<Vector2Int> diceIndices = new List<Vector2Int>();

        // North affected dice
        for (int i = 1; i < adjacentNorth + 1; i++)
        {
            int x = diceIndex.x;
            int y = diceIndex.y - i;
            diceIndices.Add(new Vector2Int(x, y));
        }

        // North East affected dice
        for (int i = 1; i < adjacentNorthEast + 1; i++)
        {
            int x = diceIndex.x + i;
            int y = diceIndex.y - i;
            diceIndices.Add(new Vector2Int(x, y));
        }

        // East affected dice
        for (int i = 1; i < adjacentEast + 1; i++)
        {
            int x = diceIndex.x + i;
            int y = diceIndex.y;
            diceIndices.Add(new Vector2Int(x, y));
        }

        // South East affected dice
        for (int i = 1; i < adjacentSouthEast + 1; i++)
        {
            int x = diceIndex.x + i;
            int y = diceIndex.y + i;
            diceIndices.Add(new Vector2Int(x, y));
        }

        // South affected dice
        for (int i = 1; i < adjacentSouth + 1; i++)
        {
            int x = diceIndex.x;
            int y = diceIndex.y + i;
            diceIndices.Add(new Vector2Int(x, y));
        }

        // South West affected dice
        for (int i = 1; i < adjacentSouthWest + 1; i++)
        {
            int x = diceIndex.x - i;
            int y = diceIndex.y + i;
            diceIndices.Add(new Vector2Int(x, y));
        }

        // West affected dice
        for (int i = 1; i < adjacentWest + 1; i++)
        {
            int x = diceIndex.x - i;
            int y = diceIndex.y;
            diceIndices.Add(new Vector2Int(x, y));
        }

        // North West affected dice
        for (int i = 1; i < adjacentNorthWest + 1; i++)
        {
            int x = diceIndex.x - i;
            int y = diceIndex.y - i;
            diceIndices.Add(new Vector2Int(x, y));
        }

        return (diceIndices, effectCaveat);
    }
}
