using UnityEngine;

public class IndicatorScript : MonoBehaviour
{
    [SerializeField] float move_dist;
    [SerializeField] float move_speed;

    Timer timer;

    Vector3 pos;

    void Start()
    {
        timer = gameObject.AddComponent<Timer>();

        timer.timer_spd = move_speed;

        pos = transform.position;
    }

    void Update()
    {
        if (timer.Toggle)
        {
            Vector3 new_pos = pos + new Vector3(0f, move_dist, 0f);

            if (new_pos != transform.position)
            {
                transform.position = new_pos;
            }
        }
        else if (transform.position != pos)
        {
            transform.position = pos;
        }
    }
}
