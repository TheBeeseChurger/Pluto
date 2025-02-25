using UnityEngine;

public class VolumeScript : MonoBehaviour
{
    static VolumeScript vol;

    private void Awake()
    {
        if (vol != null)
        {
            Destroy(gameObject);
        }

        vol = this;
        DontDestroyOnLoad(gameObject);
    }
}
