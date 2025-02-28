using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    Timer idle_timer;

    [SerializeField] float time;

    [SerializeField] AudioSource song;

    [Header("Quit")]
    [SerializeField] AudioSource ui;
    [SerializeField] AudioResource quit;

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
            
            ui.resource = quit;
            ui.time = 5f;
            ui.Play();

            Invoke(nameof(EndGame), 1.0f);
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
        song.mute = !song.mute;
    }

    private void EndGame()
    {
        #if UNITY_STANDALONE
                Application.Quit();
        #endif
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
