using UnityEngine;

public class Score
{
    public int score;
    public string name;

    public Score(int score, string name)
    {
        this.score = score;
        this.name = name.ToUpper();
    }
}
