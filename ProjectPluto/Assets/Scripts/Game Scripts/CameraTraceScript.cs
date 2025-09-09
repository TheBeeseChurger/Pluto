using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraTraceScript : MonoBehaviour
{
    [SerializeField] PlayerScript player;

    [SerializeField] float max_speed;
    [SerializeField] float max_dist;

    [SerializeField] float move_speed;

    [SerializeField] Camera cam;
    readonly Vector3 cam_default = new(0f, 0f, -10f);

    bool IsInitializing = true;
    bool IsOver = false;

    Vector3 end_pos;

    public void Init()
    {
        cam = FindFirstObjectByType<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }
    
    public void TraceStart()
    {
        IsInitializing = false;
    }

    public void TraceStop(Vector3 end_pos)
    {
        IsOver = true;

        this.end_pos = end_pos;
    }


    void FixedUpdate()
    {
        if (IsInitializing) return;

        if (!IsOver)
        {
            CamTrace(player.transform.position);
            return;
        }

        CamTrace(end_pos);
    }

    private void CamTrace(Vector3 position)
    {
        var dist = Vector2.Distance(transform.position, position);

        if (dist > max_dist)
        {
            dist = max_dist;
        }

        dist *= move_speed;

        if (dist > max_speed)
        {
            dist = max_speed;
        }

        var new_vec2 = Vector2.MoveTowards(transform.position, position, dist);

        transform.position = new Vector3((new_vec2.x), (new_vec2.y), -1f);

        cam.transform.position = new Vector3((new_vec2.x), (new_vec2.y), -10f);
    }

    public void CamReset()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        cam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cam.transform.position.z);
    }

    public void CamToOrigin()
    {
        transform.position = new Vector3(0, 0, transform.position.z);
        cam.transform.position = new Vector3(0, 0, cam.transform.position.z);
    }
}
