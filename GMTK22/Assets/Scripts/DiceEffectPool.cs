using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceEffectPool : MonoBehaviour
{
    [System.Serializable]
    public struct GenerationInfo
    {
        public int maxScore;
        public DiceEffect[] effects;
    }

    public GenerationInfo[] generations;

    public static DiceEffectPool instance;

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

    public DiceEffect GetRandomDiceEffect(int score)
    {
        for (int i = 0; i < generations.Length; i++)
        {
            GenerationInfo info = generations[i];
            if (score < info.maxScore || i == generations.Length-1)
            {
                return info.effects[Random.Range(0, info.effects.Length)];
            }
        }

        return generations[0].effects[Random.Range(0, generations[0].effects.Length)];
    }
}
