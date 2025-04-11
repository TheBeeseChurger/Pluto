using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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

    [Header("Initialization References")]
    [SerializeField] GameObject _canvas;

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

        TimerInit();

        await InstantiateAsync(_canvas);
    }

    private void Update()
    {
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
            player_name = player_name.Remove(player_name.Length - 1);
        }
    }

    public void KeyboardEnd()
    {
        while (player_name.Length < 3)
        {
            player_name = " " + player_name;
        }

        Name();

        SceneManager.LoadScene("Game");
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
