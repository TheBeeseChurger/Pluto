using UnityEngine;

public class CoreScript : MonoBehaviour
{
    static CoreScript instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
