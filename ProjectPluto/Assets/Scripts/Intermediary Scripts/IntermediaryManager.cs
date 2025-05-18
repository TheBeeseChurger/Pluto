using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IntermediaryManager : MonoBehaviour
{
    static DataScript data;

    static GameObject audio_head;

    AudioSource ui;

    [Header("Hover")]
    [SerializeField] AudioResource hover;

    [Header("Select")]
    [SerializeField] AudioResource select;

    string player_name = "";

    TextMeshProUGUI input_box;

    Timer timer;

    Button[] all_buttons;
    Button end_button;
    Button del_button;

    [Header("Initialization References")]
    [SerializeField] GameObject _canvas;

    private bool IsInitializing = true;

    public void IntermediaryStart()
    {
        FindFirstObjectByType<EventSystem>().SetSelectedGameObject(all_buttons[^1].gameObject);
    }

    public async Awaitable IntermediaryPreStart(DataScript new_data, GameObject new_audio)
    {
        if (new_audio != null)
        {
            audio_head = new_audio;
        }

        if (new_data != null)
        {
            data = new_data;
        }

        ui = audio_head.transform.GetChild(2).GetComponent<AudioSource>();

        await InstantiateAsync(_canvas);

        Init();

        TimerInit();
        IsInitializing = false;
    }

    private void Update()
    {
        if (IsInitializing) return;

        if (timer.Toggle || player_name.Length >= 3)
        {
            input_box.text = player_name;
        }
        else
        {
            input_box.text = player_name + "_";
        }

        if (player_name.Length > 0)
        {
            end_button.interactable = true;
        } else
        {
            end_button.interactable = false;
        }
    }

    private void Init()
    {
        all_buttons = FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);

        input_box = GameObject.FindGameObjectWithTag("Input").GetComponent<TextMeshProUGUI>();

        end_button = GameObject.FindGameObjectWithTag("End").GetComponent<Button>();
        del_button = GameObject.FindGameObjectWithTag("Del").GetComponent<Button>();

        foreach (Button button in all_buttons)
        {
            button.onClick.AddListener(delegate { KeyboardClick(button.name); });
        }

        end_button.onClick.RemoveAllListeners();
        del_button.onClick.RemoveAllListeners();

        end_button.onClick.AddListener(KeyboardEnd);
        del_button.onClick.AddListener(KeyboardRemove);
    }

    private void TimerInit()
    {
        timer = gameObject.AddComponent<Timer>();

        timer.timer_spd = 1;
        timer.timer_time = 0.6f;
        timer.Interrupt();
    }

    public void KeyboardClick(string key)
    {
        if (player_name.Length < 3)
        {
            player_name += key;
        }
    }

    public void KeyboardRemove()
    {
        if (player_name.Length > 0)
        {
            player_name = player_name[..^1];
        }
    }

    public void KeyboardEnd()
    {
        end_button.onClick.RemoveAllListeners();

        while (player_name.Length < 3)
        {
            player_name = " " + player_name;
        }

        Name();

        ProjectManager.IntermediaryToGame.Invoke();
    }

    public void UISelectSFX()
    {
        ui.resource = hover;
        ui.Play();
    }

    public void UIPressSFX()
    {
        ui.resource = select;
        ui.Play();
    }

    private void Name()
    {
        data.SetCurrent(new Score(0, player_name));
    }
}
