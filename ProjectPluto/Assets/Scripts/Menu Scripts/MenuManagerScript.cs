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
    InputAction _controls;

    GameObject controls_menu;

    [SerializeField] float time;

    static GameObject audio_head;
    static int game_start = 0;
    static DataScript data;

    AudioSource song;
    AudioSource ui;

    [Header("AudioResources")]
    [SerializeField] AudioResource quit;
    [SerializeField] AudioResource bgm;

    [Header("Initialization References")]
    [SerializeField] GameObject _canvas;

    IndicatorScript _indicator_script;
    ScoreManagerScript _score_manager_script;
    BlinkTextScript _blink_text_script;

    [Header("Intro Animation")]
    [SerializeField] GameObject anim_intro;

    private bool IsInitializing = true;

    public async Awaitable MenuPreStart(DataScript new_data, GameObject new_audio)
    {
        game_start++;
        if (new_audio != null)
        {
            audio_head = new_audio;
        }

        if (new_data != null)
        {
            data = new_data;
        }

        Init();

        await InstantiateAsync(anim_intro);
        await InstantiateAsync(_canvas);
        _indicator_script = FindAnyObjectByType<IndicatorScript>();

        _indicator_script.Init();

        _score_manager_script = FindAnyObjectByType<ScoreManagerScript>();

        data = _score_manager_script.DataInit(data);
        _score_manager_script.Init();
        
        _blink_text_script = FindAnyObjectByType<BlinkTextScript>();

        _blink_text_script.Init();

        controls_menu = GameObject.FindGameObjectWithTag("Controls");
        controls_menu.SetActive(false);
    }

    public void MenuStart()
    {
        _score_manager_script.Start();

        song.resource = bgm;
        song.volume = 0.8f;
        song.loop = true;

        GameManager.gameTimeScale = 1f;
        
        if (game_start == 1)
        {
            song.PlayDelayed(5.0f);
            FindAnyObjectByType<Animator>().Play("Open");
        } else
        {
            song.Play();
            FindAnyObjectByType<Animator>().Play("Screen Fade");
        }
            
    }

    private void Init()
    {
        song = audio_head.transform.GetChild(0).GetComponent<AudioSource>();
        ui = audio_head.transform.GetChild(2).GetComponent<AudioSource>();

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

        _controls = _input_system.UI.Controls;
        _controls.Enable();

        _controls.performed += Controls;
    }

    private void DisableActions()
    {
        _cancel?.Disable();
        _mute?.Disable();
        _play?.Disable();
        _controls?.Disable();
    }

    private void OnDisable()
    {
        DisableActions();
    }

    private void Cancel(InputAction.CallbackContext _context)
    {
        ui.resource = quit;
        ui.time = 5.1f;
        ui.Play();

        FindFirstObjectByType<Animator>().Play("Close");
        song.Stop();
        Invoke(nameof(EndGame), 1.0f);
    }

    private void Mute(InputAction.CallbackContext _context)
    {
        ToggleSong();
    }

    private void Play(InputAction.CallbackContext _context)
    {
        DisableActions();
        ProjectManager.MenuToIntermediary?.Invoke();
    }

    public void ToggleSong()
    {
        song.mute = !song.mute;
    }

    private void Controls(InputAction.CallbackContext _context)
    {
        controls_menu.SetActive(!controls_menu.activeSelf);
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
