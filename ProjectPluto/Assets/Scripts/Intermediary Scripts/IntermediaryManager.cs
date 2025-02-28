using TMPro;
using UnityEngine;

public class IntermediaryManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] GameObject prefab;
    DataScript data;

    string player_name;

    [SerializeField] TextMeshProUGUI input_box;

    void Awake()
    {
        GameObject dataobj = GameObject.FindWithTag("data");

        if (dataobj == null)
        {
            dataobj = GameObject.Instantiate(prefab);
        }

        data = dataobj.GetComponent<DataScript>();
    }

    private void Update()
    {
        input_box.text = player_name;
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

    public void Name()
    {
        data.SetCurrent(new Score(0, player_name));
    }
}
