using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntermediaryManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] GameObject prefab;
    static DataScript data;

    string player_name = "";

    [Header("Input Box")]
    [SerializeField] TextMeshProUGUI input_box;

    Timer timer;

    [Header("END Button")]
    [SerializeField] Button end_button;

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

    private void Name()
    {
        data.SetCurrent(new Score(0, player_name));
    }
}
