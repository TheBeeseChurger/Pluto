using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    Timer idle_timer;

    void Start()
    {
        idle_timer = gameObject.AddComponent<Timer>();

        idle_timer.timer_spd = 1f;
        idle_timer.timer_time = 5f;
        idle_timer.Interrupt();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // Play a sound?
            Application.Quit();
        }

        if (idle_timer.end)
        {
            SceneManager.LoadScene("GameIdle");
        }
    }

    public void Click()
    {
        SceneManager.LoadScene("Game");
    }
}
