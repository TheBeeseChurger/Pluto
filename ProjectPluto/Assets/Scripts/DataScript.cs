 using UnityEngine;

public class DataScript : MonoBehaviour
{
    static DataScript script;

    public HiScoreBoard data = new();

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
