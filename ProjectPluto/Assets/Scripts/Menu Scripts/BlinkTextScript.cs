using UnityEngine;
using TMPro;

public class BlinkTextScript : MonoBehaviour
{
    [SerializeField] float blink_spd;
    TextMeshProUGUI text;

    Timer timer;

    private bool IsInitializing = true;

    public void Init()
    {
        text = GetComponent<TextMeshProUGUI>();

        timer = gameObject.AddComponent<Timer>();

        timer.timer_spd = blink_spd;

        IsInitializing = false;
    }

    void Update()
    {
        if (IsInitializing) return;

        text.enabled = timer.Toggle;
    }

}
