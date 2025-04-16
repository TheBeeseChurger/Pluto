using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    GameObject cam;

    bool IsInitializing = true;

    public void Init(bool OnOff = true)
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");

        IsInitializing = !OnOff;
    }
    
    void Update()
    {
        if (IsInitializing) return;

        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
    }
}
