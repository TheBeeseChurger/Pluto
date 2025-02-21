using System.Collections.Generic;
using UnityEngine;

public class HiScoreBoard
{
    static int newest_score = -1;

    private List<Score> board = new();

    void AddScore(Score _score)
    {
        int i = 0;

        foreach (var row in board)
        {
            if (row.score < _score.score)
            {
                board.Insert(i, _score);
                newest_score = i;
                break;
            }
            else i++;
        }

        if (board.Count < i)
        {
            board.Add(_score);
        }

        if (board.Count > 10)
        {
            board.RemoveAt(10);
        }
    }

    void AddScore(string name, int _score)
    {
        AddScore(new Score(_score, name));
    }

    Score GetScore(int index)
    {
        return board[index];
    }
}
