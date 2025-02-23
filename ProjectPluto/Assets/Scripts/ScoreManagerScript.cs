using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class ScoreManagerScript : MonoBehaviour
{
    [SerializeField] float scroll_spd = 1f;
    [SerializeField] float scroll_delay = 0.5f;

    private bool scrolling = true;

    Vector3 dir;

    Timer timer;

    [Header("Data")]
    [SerializeField] GameObject prefab;
    DataScript data;
    void Awake()
    {
        GameObject dataobj = GameObject.FindWithTag("data");

        if (dataobj == null)
        {
            dataobj = GameObject.Instantiate(prefab);
        }

        data = dataobj.GetComponent<DataScript>();
    }
    void Start()
    {
        dir = new Vector3(0f, scroll_spd, 0f);

        timer = gameObject.AddComponent<Timer>();

        timer.timer_spd = 2;

        int pos = 9;

        foreach (ScoreTextScript child in gameObject.GetComponentsInChildren<ScoreTextScript>())
        {
            child.transform.localPosition = new Vector3(0f, pos, 0f);
            pos += -2;
        }
    }

    void Update()
    {
        if (!scrolling)
        {
            if (!timer.end) return;

            timer.timer_spd = 2;
            timer.Interrupt();
        }

        if (timer.end)
        {
            transform.localPosition += dir;

            if (transform.localPosition.y > 7f)
            {
                scrolling = false;
                dir = new Vector3(0f, -scroll_spd, 0f);

                transform.localPosition = new Vector3(0f, 7f, 0f);

                timer.timer_spd = scroll_delay;
                timer.Interrupt();
            }

            if (transform.localPosition.y < -7f)
            {
                scrolling = false;
                dir = new Vector3(0f, scroll_spd, 0f);

                transform.localPosition = new Vector3(0f, -7f, 0f);
                timer.timer_spd = scroll_delay;
                timer.Interrupt();
            }
        }
    }
}
