using System.Collections.Generic;
using UnityEngine;

public class HiScoreBoard
{
    static int newest_score = -1;

    private List<Score> board = new();

    public void AddScore(Score _score)
    {
        int i = 0;

        foreach (var row in board)
        {
            if (row.GetScoreRaw() < _score.GetScoreRaw())
            {
                board.Insert(i, _score);
                newest_score = i;
                break;
            }
            else i++;
        }

        if (board.Count == i)
        {
            board.Add(_score);
            newest_score = i;
        }

        if (board.Count > 10)
        {
            board.RemoveAt(10);
        }
    }

    public void AddScore(string name, int _score)
    {
        AddScore(new Score(_score, name));
    }

    public Score GetScore(int index)
    {
        if (board.Count <= index)
        {
            return new Score(0, "AAA");
        }

        return board[index];
    }

    public bool IsRecent(int index)
    {
        return newest_score == index;
    }
}
