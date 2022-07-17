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
        public bool allowDuplicateNormals;
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

    public DiceEffect GetRandomDiceEffect(int score, ref bool allowDuplicateNormals)
    {
        for (int i = 0; i < generations.Length; i++)
        {
            GenerationInfo info = generations[i];
            if (score < info.maxScore || i == generations.Length-1)
            {
                allowDuplicateNormals = info.allowDuplicateNormals;
                return info.effects[Random.Range(0, info.effects.Length)];
            }
        }

        allowDuplicateNormals = generations[0].allowDuplicateNormals;
        return generations[0].effects[Random.Range(0, generations[0].effects.Length)];
    }
}
