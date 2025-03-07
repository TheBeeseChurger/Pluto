using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GeneratorScript maze_gen;
    [SerializeField] CameraTraceScript tracer;

    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject evil;

    [SerializeField] TextMeshProUGUI score_text;
    private static int score = 110;

    private Timer timer;

    private int x_pos;
    private int y_pos;

    [Header("Data")]
    [SerializeField] GameObject prefab;
    DataScript data;

    private void Awake()
    {
        GameObject dataobj = GameObject.FindWithTag("data");

        if (dataobj == null)
        {
            dataobj = GameObject.Instantiate(prefab);
        }

        data = dataobj.GetComponent<DataScript>();
        
        timer = gameObject.AddComponent<Timer>();

        timer.timer_spd = 1f;
        timer.timer_time = 1f;
    }

    void Start()
    {
        maze_gen.Generate();

        var p1_cell = maze_gen.GetPlayer1Spawn();
        var p2_cell = maze_gen.GetPlayer2Spawn();
        var evil_cell = maze_gen.GetEvilSpawn();

        player1.transform.position = new Vector3(p1_cell.transform.position.x,p1_cell.transform.position.y, player1.transform.position.z);
        player2.transform.position = new Vector3(p2_cell.transform.position.x, p2_cell.transform.position.y, player2.transform.position.z);
        evil.transform.position = new Vector3(evil_cell.transform.position.x, evil_cell.transform.position.y, evil.transform.position.z);

        tracer.CamReset();

        var start_pos = CalcMazePos(player1.transform.position);

        x_pos = (int)start_pos.x;
        y_pos = (int)start_pos.y;
    }

    void Update()
    {
        if (timer.End && score >= 10)
        {
            score -= 10;
        }

        score_text.text = "Score:" + score;

        var curr_pos = CalcMazePos(player1.transform.position);

        if (x_pos != (int)curr_pos.x || y_pos != (int)curr_pos.y)
        {
            x_pos = (int)curr_pos.x;
            y_pos = (int)curr_pos.y;

            if (!maze_gen.GetCell(x_pos, y_pos).IsSeen)
            {
                maze_gen.GetCell(x_pos, y_pos).See();

                if (maze_gen.GetCell(x_pos, y_pos).IsLandmarkCell)
                {
                    score += 100;
                } else
                {
                    score += 10;
                }
            }
        }
    }

    public void EndGame()
    {
        data.AddScore(score);
        data.CommitScore();

        SceneManager.LoadScene("Menu");
        //END GAME
    }

    public void NextRound()
    {
        score += 500;

        data.AddScore(score);

        SceneManager.LoadScene("Game");
    }

    private Vector2 CalcMazePos(Vector3 pos)
    {
        return CalcMazePos(pos.x, pos.y);
    }

    private Vector2 CalcMazePos(float x, float y)
    {
        return new Vector2(x - maze_gen.transform.position.x + 0.5f, y - maze_gen.transform.position.y + 0.5f);
    }
}
