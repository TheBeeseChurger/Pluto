using UnityEngine;
using TMPro;
public class ScoreTextScript : MonoBehaviour
{
    public Score score;

    TextMeshProUGUI text;

    public int rank;

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void FormatText()
    {

        text.text = score.GetName() + score.GetScore();
    }
}
