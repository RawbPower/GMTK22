using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DiceEffect : ScriptableObject
{
    public string effectName;
    public Sprite[] diceFaces;
    public Color tint = Color.white;

    public virtual List<Vector2Int> GetEffectIndices(Vector2Int diceIndex)
    {
        List<Vector2Int> diceIndices = new List<Vector2Int>();

        return diceIndices;
    }

    public virtual string GetName()
    {
        return effectName;
    }
}
