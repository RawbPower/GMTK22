using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DiceEffect : ScriptableObject
{
    [System.Serializable]
    public enum EffectCaveat
    {
        NONE,
        ODD_EVEN,
        SAME_FACE
    }

    public string effectName;
    public Sprite[] diceFaces;
    public Color tint = Color.white;
    public EffectCaveat effectCaveat;

    public virtual (List<Vector2Int>, EffectCaveat) GetEffectIndices(Vector2Int diceIndex)
    {
        List<Vector2Int> diceIndices = new List<Vector2Int>();

        return (diceIndices, effectCaveat);
    }

    public virtual string GetName()
    {
        return effectName;
    }
}
