using UnityEngine;

public class Timer : MonoBehaviour
{
    const float DEFAULT_TIME = 1.0f;

    public float timer_spd;

    float curtime = DEFAULT_TIME;

    public bool toggle = true;

    public bool end = false;

    void Update()
    {
        if (timer_spd == 0) return;

        curtime -= Time.deltaTime;
        end = false;

        if (curtime <= 0)
        {
            toggle = !toggle;
            end = true;

            curtime = DEFAULT_TIME / timer_spd;
        }
    }

    public void Interrupt()
    {
        if (timer_spd == 0) return;

        toggle = true;
        curtime = DEFAULT_TIME / timer_spd;
    }
}
