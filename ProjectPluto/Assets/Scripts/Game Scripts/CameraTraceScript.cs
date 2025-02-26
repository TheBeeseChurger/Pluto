using System;
using UnityEngine;

public class CameraTraceScript : MonoBehaviour
{
    [SerializeField] PlayerScript player;

    [SerializeField] float max_speed;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        var dist = Vector2.Distance(transform.position, player.transform.position) / 10f;

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, player.transform.position, 0.4f);
    }
}
