using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time;
    public GameObject timerUI;

    public delegate void TimerEnd();
    public static event TimerEnd OnTimerEnd;

    private float timePassed;
    private bool timerStarted;
    // Start is called before the first frame update
    void Awake()
    {
        timePassed = 0.0f;
        timerStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerStarted)
        {
            timePassed += Time.deltaTime;
            if (timePassed > time)
            {
                timePassed = time;
                timerStarted = false;
                OnTimerEnd();
            }
        }

        if (timerUI)
        {
            timerUI.GetComponent<TMPro.TextMeshProUGUI>().text = GetTimeLeft().ToString();
        }

    }

    public void StartTimer()
    {
        timePassed = 0.0f;
        timerStarted = true;
    }

    public int GetTimeLeft()
    {
        return Mathf.CeilToInt(time - timePassed);
    }
}
