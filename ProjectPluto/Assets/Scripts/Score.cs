using System;
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

    public string GetName()
    {
        return this.name;
    }

    public string GetScore()
    {
        return this.score.ToString().PadLeft(20, '.');
    }
}
