 using UnityEngine;

public class DataScript : MonoBehaviour
{
    static DataScript script;

    HiScoreBoard data;
    void Awake()
    {
        if (script != null)
        {
            Destroy(gameObject);
        }

        script = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
