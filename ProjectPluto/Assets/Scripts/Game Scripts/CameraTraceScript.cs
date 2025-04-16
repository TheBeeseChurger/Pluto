using System;
using UnityEngine;

public class CameraTraceScript : MonoBehaviour
{
    [SerializeField] PlayerScript player;

    [SerializeField] float max_speed;
    [SerializeField] float max_dist;

    [SerializeField] float move_speed;

    [SerializeField] Camera cam;
    readonly Vector3 cam_default = new(0f, 0f, -10f);

    bool IsInitializing = true;

    public void Init()
    {
        cam = FindFirstObjectByType<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }
    
    public void TraceStart()
    {
        IsInitializing = false;
    }

    public void TraceStop()
    {
        IsInitializing = true;

        cam.transform.localPosition = cam_default;
    }


    void FixedUpdate()
    {
        if (IsInitializing) return;

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

        transform.position = new Vector3((new_vec2.x) * GameManager.gameTimeScale, (new_vec2.y) * GameManager.gameTimeScale, -1f);

        cam.transform.position = new Vector3((new_vec2.x) * GameManager.gameTimeScale, (new_vec2.y) * GameManager.gameTimeScale, -10f);
    }

    public void CamReset()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        cam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cam.transform.position.z);
    }
}
