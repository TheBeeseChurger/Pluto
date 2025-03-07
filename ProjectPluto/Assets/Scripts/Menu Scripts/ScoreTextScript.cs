using UnityEngine;
using TMPro;
public class ScoreTextScript : MonoBehaviour
{
    public Score score = new Score(0, "AAA");

    TextMeshProUGUI text;

    public int rank;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void FormatText()
    {

        text.text = score.GetName() + score.GetScore();
    }
}
