using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    Timer idle_timer;
    InputSystem_Actions _input_system;
    InputAction _cancel;
    InputAction _mute;
    InputAction _play;

    [SerializeField] float time;

    static GameObject audio_head;

    static DataScript data;

    AudioSource song;
    AudioSource ui;

    [Header("AudioResources")]
    [SerializeField] AudioResource quit;
    [SerializeField] AudioResource bgm;

    [Header("Initialization References")]
    [SerializeField] GameObject _canvas;
    [SerializeField] IndicatorScript _indicator_script;
    [SerializeField] ScoreManagerScript _score_manager_script;
    [SerializeField] BlinkTextScript _blink_text_script;

    private bool IsInitializing = true;

    public (DataScript my_data, GameObject my_audio) GetStatics()
    {
        return (data, audio_head);
    }

    public async Awaitable MenuStart(DataScript new_data, GameObject new_audio)
    {
        if (new_audio != null)
        {
            audio_head = new_audio;
        }

        if (new_data != null)
        {
            data = new_data;
        }

        await Init();

        await InstantiateAsync(_canvas);
        _indicator_script = FindAnyObjectByType<IndicatorScript>();

        _indicator_script.Init();

        _score_manager_script = FindAnyObjectByType<ScoreManagerScript>();

        data = _score_manager_script.DataInit(data);
        _score_manager_script.Init();
        
        _blink_text_script = FindAnyObjectByType<BlinkTextScript>();

        _blink_text_script.Init();
    }

    private async Awaitable Init()
    {
        await Awaitable.MainThreadAsync();
        song = audio_head.transform.GetChild(0).GetComponent<AudioSource>();
        ui = audio_head.transform.GetChild(2).GetComponent<AudioSource>();

        song.resource = bgm;
        song.loop = true;
        song.volume = 1.0f;
        song.Play();

        idle_timer = gameObject.AddComponent<Timer>();

        idle_timer.timer_spd = 1f;
        idle_timer.timer_time = time;
        idle_timer.Interrupt();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _input_system = new();
        EnableActions();

        IsInitializing = false;
    }

    void Update()
    {
        if (IsInitializing) return;

        if (idle_timer.End)
        {
            SceneManager.LoadScene("GameIdle");
        }
    }

    private void EnableActions()
    {
        _cancel = _input_system.UI.Cancel;
        _cancel.Enable();

        _cancel.performed += Cancel;

        _mute = _input_system.UI.Mute;
        _mute.Enable();

        _mute.performed += Mute;

        _play = _input_system.UI.Submit;
        _play.Enable();

        _play.performed += Play;
    }

    private void OnDisable()
    {
        _cancel.Disable();
        _mute.Disable();
        _play.Disable();
    }

    private void Cancel(InputAction.CallbackContext _context)
    {
        ui.resource = quit;
        ui.time = 5f;
        ui.Play();

        Invoke(nameof(EndGame), 1.0f);
    }

    private void Mute(InputAction.CallbackContext _context)
    {
        ToggleSong();
    }

    private void Play(InputAction.CallbackContext _context)
    {
        SceneManager.LoadScene("Intermediary");
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
