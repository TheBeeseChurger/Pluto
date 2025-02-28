using System;
using UnityEngine;

public class Score
{
    int score;
    string name;

    public Score(int score, string name)
    {
        this.score = score;
        this.name = name.ToUpper();
    }

    public string GetName()
    {
        return this.name;
    }

    public string GetScore()
    {
        return this.score.ToString().PadLeft(21, '.');
    }

    public int GetScoreRaw()
    {
        return this.score;
    }

    public void SetScore(int score)
    {
        this.score = score;
    }
}
