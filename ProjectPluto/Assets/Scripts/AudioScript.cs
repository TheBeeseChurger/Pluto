using UnityEngine;

public class AudioScript : MonoBehaviour
{
    static AudioScript script;


    void Awake()
    {
        if (script != null)
        {
            Destroy(gameObject);
        }

        script = this;
        DontDestroyOnLoad(gameObject);
    }
}
