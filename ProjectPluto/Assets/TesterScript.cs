using System;
using UnityEngine;

public class TesterScript : MonoBehaviour
{
    public GameObject tested_prefab;

    private GeneratorScript gen;

    private bool playing;

    private Timer timer;

    void Start()
    {
        gen = Instantiate(tested_prefab).GetComponent<GeneratorScript>();

        gen.Generate();

        timer = gameObject.AddComponent<Timer>();

        timer.timer_spd = 1f;
        timer.timer_time = 0.25f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playing = !playing;
        }

        if (timer.End && playing)
        {
            gen.DeleteRandomCell();
        }
    }
}
