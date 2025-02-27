using System;
using UnityEngine;

public class CameraTraceScript : MonoBehaviour
{
    [SerializeField] PlayerScript player;

    [SerializeField] float max_speed;
    [SerializeField] float max_dist;

    [SerializeField] float move_speed;

    [SerializeField] Camera cam;

    void FixedUpdate()
    {
        var dist = Vector2.Distance(transform.position, player.transform.position);

        dist *= move_speed;

        if (dist > max_dist)
        {
            dist *= 1.2f;
        }
        else if (dist > max_speed)
        {
            dist = max_speed;
        }

        var new_vec2 = Vector2.MoveTowards(transform.position, player.transform.position, dist);

        transform.position = new Vector3(new_vec2.x, new_vec2.y, -1f);

        cam.transform.position = new Vector3(new_vec2.x, new_vec2.y, -10f);
    }
}
