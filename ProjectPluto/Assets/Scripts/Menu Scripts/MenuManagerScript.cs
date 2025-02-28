using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    Timer idle_timer;

    [SerializeField] float time;

    [SerializeField] AudioSource song;

    void Start()
    {
        idle_timer = gameObject.AddComponent<Timer>();

        idle_timer.timer_spd = 1f;
        idle_timer.timer_time = time;
        idle_timer.Interrupt();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // Play a sound?
            Application.Quit();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SceneManager.LoadScene("Game");
        }

        if (idle_timer.end)
        {
            SceneManager.LoadScene("GameIdle");
        }
    }

    public void ToggleSong()
    {
        song.enabled = !song.enabled;
    }
}
