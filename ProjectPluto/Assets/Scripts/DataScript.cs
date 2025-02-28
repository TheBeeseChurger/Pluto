 using UnityEngine;

public class DataScript : MonoBehaviour
{
    static DataScript script;

    public HiScoreBoard data = new();

    private Score curr_score = null;

    void Awake()
    {
        if (script != null)
        {
            Destroy(gameObject);
        }

        script = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCurrent(Score score)
    {
        curr_score = score;
    }

    public void AddScore(int score)
    {
        curr_score?.SetScore(curr_score.GetScoreRaw() + score);
    }

    public void CommitScore()
    {
        data.AddScore(curr_score);
    }
}
