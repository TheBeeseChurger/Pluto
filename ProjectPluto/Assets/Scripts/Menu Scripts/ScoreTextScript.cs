using UnityEngine;
using TMPro;
public class ScoreTextScript : MonoBehaviour
{
    public Score score = new(0, "AAA");

    TextMeshProUGUI text;

    public int rank;

    public void Init()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void FormatText()
    {
        text.text = score.GetName() + score.GetScore();
    }
}
