using UnityEngine;
using TMPro;

public class BlinkTextScript : MonoBehaviour
{
    [SerializeField] float blink_spd;
    TextMeshProUGUI text;

    Timer timer;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        timer = gameObject.AddComponent<Timer>();

        timer.timer_spd = blink_spd;
    }

    void Update()
    {
        text.enabled = timer.toggle;
    }

}
