using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    GameObject cam;

    bool IsInitializing = true;

    public void Init()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");

        IsInitializing = false;
    }
    
    void Update()
    {
        if (IsInitializing) return;

        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
    }
}
