using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntermediaryManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] GameObject prefab;
    static DataScript data;

    [Header("Audio")]
    [SerializeField] GameObject a_prefab;
    static GameObject audio_head;

    AudioSource ui;

    [Header("Hover")]
    [SerializeField] AudioResource hover;

    [Header("Select")]
    [SerializeField] AudioResource select;

    string player_name = "";

    [Header("Input Box")]
    [SerializeField] TextMeshProUGUI input_box;

    Timer timer;

    [Header("END Button")]
    [SerializeField] Button end_button;

    [Header("Keys")]
    {[SerializeField] Button[] buttons;

    void Awake()
    {
        if (data == null)
        {
            GameObject dataobj = GameObject.FindWithTag("data");

            if (dataobj == null)
            {
                dataobj = GameObject.Instantiate(prefab);
            }

            data = dataobj.GetComponent<DataScript>();
        }

        if (audio_head == null)
        {
            audio_head = GameObject.FindGameObjectWithTag("audio");

            if (audio_head == null)
            {
                audio_head = Instantiate(prefab);
            }
        }

        ui = audio_head.transform.GetChild(2).GetComponent<AudioSource>();

        foreach (var button in buttons)
        {
            button.onClick.AddListener(UISelectSFX);
        }

        timer = gameObject.AddComponent<Timer>();

        timer.timer_spd = 1;
        timer.timer_time = 0.6f;
        timer.Interrupt();
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

    private void UIHoverSFX()
    {
        ui.resource = hover;
        ui.Play();
    }

    private void UISelectSFX()
    {
        ui.resource = select;
        ui.Play();
    }

    private void Name()
    {
        data.SetCurrent(new Score(0, player_name));
    }
}
