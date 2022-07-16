using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceEffectPool : MonoBehaviour
{
    public static DiceEffectPool instance;

    public DiceEffect[] effects;

    // Start is called before the first frame update
    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public DiceEffect GetRandomDiceEffect()
    {
        return effects[Random.Range(0, effects.Length)];
    }
}
