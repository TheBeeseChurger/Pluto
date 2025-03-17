using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    Timer idle_timer;

    [SerializeField] float time;

    [Header("Audio")]
    [SerializeField] GameObject prefab;
    static GameObject audio_head;

    AudioSource song;
    AudioSource ui;

    [Header("Quit")]
    [SerializeField] AudioResource quit;

    void Start()
    {
        if (audio_head == null)
        { 
            audio_head = GameObject.FindGameObjectWithTag("audio");

            if (audio_head == null)
            {
                audio_head = Instantiate(prefab);
            }
        }

        song = audio_head.transform.GetChild(0).GetComponent<AudioSource>();
        ui = audio_head.transform.GetChild(2).GetComponent<AudioSource>();

        idle_timer = gameObject.AddComponent<Timer>();

        idle_timer.timer_spd = 1f;
        idle_timer.timer_time = time;
        idle_timer.Interrupt();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            SceneManager.LoadScene("Intermediary");
        }

        if (idle_timer.End)
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
