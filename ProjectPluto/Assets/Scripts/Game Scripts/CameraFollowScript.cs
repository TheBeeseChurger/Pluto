using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    GameObject cam;

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }
    
    void Update()
    {
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
    }
}
