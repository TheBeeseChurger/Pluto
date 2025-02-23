using UnityEngine;
using TMPro;
public class ScoreTextScript : MonoBehaviour
{
    Score my_score;

    TextMeshProUGUI text;

    public int rank;
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        
    }
}
