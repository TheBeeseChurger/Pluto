using UnityEngine;

public class Timer : MonoBehaviour
{
    const float DEFAULT_TIME = 1.0f;

    public float timer_spd;
    public float timer_time;

    float curtime = DEFAULT_TIME;

    public bool Toggle { get; private set; } = true;

    public bool End { get; private set; } = false;

    void Update()
    {
        if (timer_spd == 0) return;

        curtime -= (Time.deltaTime) * GameManager.gameTimeScale;
        End = false;

        if (curtime <= 0)
        {
            Toggle = !Toggle;
            End = true;

            if (timer_time == 0)
            {
                curtime = DEFAULT_TIME / timer_spd;
            }
            else
            {
                curtime = timer_time / timer_spd;
            }
            
        }
    }

    public void Interrupt()
    {
        if (timer_spd == 0) return;

        Toggle = true;

        if (timer_time == 0)
        {
            curtime = DEFAULT_TIME / timer_spd;
        }
        else
        {
            curtime = timer_time / timer_spd;
        }
    }
}
