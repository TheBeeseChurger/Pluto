using System.Security;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;

    private float horizontal;
    private float vertical;

    [SerializeField] private float move_speed;

    Timer timer;

    private bool ready = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        timer = gameObject.AddComponent<Timer>();

        timer.timer_spd = 2f;
        timer.timer_time = 1f;
        

        timer.Interrupt();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (timer.end)
        {
            ready = true;
        }

        if ((horizontal != 0 || vertical != 0) && ready)
        {
            ready = false;
            transform.position += new Vector3(horizontal, vertical, 0f);
            timer.Interrupt();
        }
    }

}
