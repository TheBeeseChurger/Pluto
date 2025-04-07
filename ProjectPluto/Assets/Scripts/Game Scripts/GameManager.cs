using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GeneratorScript maze_gen;
    [SerializeField] CameraTraceScript tracer;

    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject evil;

    [SerializeField] GameObject p2compass_hand;
    [SerializeField] GameObject evcompass_hand;

    [SerializeField] TextMeshProUGUI score_text;
    [SerializeField] TextMeshProUGUI multiplier_text;
    private static int score = 110;
    private static float score_multiplier = 1f;

    private Timer score_timer;
    private Timer distance_timer;

    private int last_distance;

    private int x_pos;
    private int y_pos;

    private bool compass_jammer = false;

    [Header("Data")]
    [SerializeField] GameObject prefab;
    static DataScript data;

    [Header("Audio")]
    [SerializeField] GameObject a_prefab;
    static GameObject audio_head;

    AudioSource song;
    AudioSource ui;

    [Header("AudioResource")]
    [SerializeField] AudioResource bgm;

    private void Awake()
    {
        if (data == null)
        {
            GameObject dataobj = GameObject.FindWithTag("data");

            if (dataobj == null)
            {
                dataobj = GameObject.Instantiate(prefab);
            }

            data = dataobj.GetComponent<DataScript>();
        }

        if (audio_head == null)
        {
            audio_head = GameObject.FindGameObjectWithTag("audio");

            if (audio_head == null)
            {
                audio_head = Instantiate(a_prefab);
            }
        }

        song = audio_head.transform.GetChild(0).GetComponent<AudioSource>();
        ui = audio_head.transform.GetChild(2).GetComponent<AudioSource>();

        if (song.resource != bgm)
        {
            song.resource = bgm;
            song.loop = true;
            song.volume = 0.45f;
            song.Play();
        }

        score_timer = gameObject.AddComponent<Timer>();

        score_timer.timer_spd = 1f;
        score_timer.timer_time = 1f;

        distance_timer = gameObject.AddComponent<Timer>();

        distance_timer.timer_spd = 1f;
        distance_timer .timer_time = 5f;
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

        if (!maze_gen.GetCell(x_pos, y_pos).IsLandmarkCell)
        {
            maze_gen.GetCell(x_pos, y_pos).See();
        }
        else
        {
            maze_gen.GetCell(x_pos, y_pos).GetComponentInParent<LandmarkCellScript>().See();
        }

        List<MazeCellScript> cells = maze_gen.GetStraightConnectedCells(maze_gen.GetCell(x_pos, y_pos));

        foreach (var cell in cells)
        {
            if (!cell.IsSeen)
            {
                SeeAndScore(cell);
            }
        }

        last_distance = maze_gen.GetCellDistance(p1_cell, p2_cell);
        //Debug.Log("Player spawned " + last_distance + " tiles away from Player 2.");
        distance_timer.Interrupt();
    }

    void Update()
    {
        if (score_timer.End && score >= 10)
        {
            score -= 10;
        }

        if (distance_timer.End)
        {
            last_distance = maze_gen.GetCellDistance(CalcMazePos(player1.transform.position), CalcMazePos(player2.transform.position));

            //Debug.Log("Updated distance to " + last_distance);

            if (last_distance < 3)
            {
                NextRound();
            }
            else if (last_distance < 10)
            {
                distance_timer.timer_time = 0f;
                distance_timer.Interrupt();
                //Debug.Log("Distance was less than 10, updating every frame now.");
            }
            else if (last_distance < 25)
            {
                distance_timer.timer_time = 1f;
                //Debug.Log("Distance was less than 25, updating distance faster.");
                distance_timer.Interrupt();
            }
            else if (last_distance < 40)
            {
                distance_timer.timer_time = 2.5f;
                distance_timer.Interrupt();
                //Debug.Log("Distance was less than 40, updating distance a bit faster.");
            }
            else
            {
                distance_timer.timer_time = 5f;
                distance_timer.Interrupt();
                //Debug.Log("Distance greater than 40, updating distance slowly.");
            }
        }

        score_text.text = "Score:" + score;
        multiplier_text.text = "Multipler x" + score_multiplier;

        var curr_pos = CalcMazePos(player1.transform.position);

        if (x_pos != (int)curr_pos.x || y_pos != (int)curr_pos.y)
        {
            x_pos = (int)curr_pos.x;
            y_pos = (int)curr_pos.y;

            if (!maze_gen.GetCell(x_pos, y_pos).IsSeen)
            {
                SeeAndScore(maze_gen.GetCell(x_pos, y_pos));
            }

            List<MazeCellScript> cells = maze_gen.GetStraightConnectedCells(maze_gen.GetCell(x_pos, y_pos));

            foreach (var cell in cells)
            {
                if (!cell.IsSeen)
                {
                    SeeAndScore(cell);
                }
            }
        }

        CompassControl();
    }

    public void EndGame()
    {
        data.AddScore(score);
        data.CommitScore();
        ResetScore();

        SceneManager.LoadScene("Menu");
        //END GAME
    }

    public void NextRound()
    {
        score += (int)(500 * score_multiplier);
        score_multiplier += 0.5f;

        SceneManager.LoadScene("Game");
    }
    
    public List<Vector2> PossibleDirections(Vector2 curr_pos)
    {
        List<Vector2> result = new();

        var my_pos = CalcMazePos(curr_pos);
        var my_cell = maze_gen.GetCell((int)my_pos.x, (int)my_pos.y);

        var connections = maze_gen.GetConnectedCells(my_cell);

        foreach (var connection in connections)
        {
            var cell_pos = connection.transform.position;

            if (cell_pos.x < my_cell.transform.position.x - 0.01f)
            {
                //Debug.Log("Adding Left direction as possible");
                result.Add(Vector2.left);
            }
            else if (cell_pos.x > my_cell.transform.position.x + 0.01f)
            {
                //Debug.Log("Adding Right direction as possible");
                result.Add(Vector2.right);
            }
            else if (cell_pos.y < my_cell.transform.position.y - 0.01f)
            {
                //Debug.Log("Adding Down direction as possible");
                result.Add(Vector2.down);
            }
            else if (cell_pos.y > my_cell.transform.position.y + 0.01f)
            {
                //Debug.Log("Adding Up direction as possible");
                result.Add(Vector2.up);
            }
        }

        return result;
    }

    private Vector2 CalcMazePos(Vector3 pos)
    {
        return CalcMazePos(pos.x, pos.y);
    }

    private Vector2 CalcMazePos(Vector2 pos)
    {
        return CalcMazePos(pos.x, pos.y);
    }

    private Vector2 CalcMazePos(float x, float y)
    {
        return new Vector2(x - maze_gen.transform.position.x + 0.5f, y - maze_gen.transform.position.y + 0.5f);
    }

    private void ResetScore()
    {
        score = 110;
        score_multiplier = 1;
    }

    private void SeeAndScore(MazeCellScript cell)
    {
        cell.See();

        if (cell.IsLandmarkCell)
        {
            score += (int)(100 * score_multiplier);
        }
        else
        {
            score += (int)(10 * score_multiplier);
        }
    }

    private void CompassControl()
    {
        var dist1 = player2.transform.position - player1.transform.position;
        var dist2 = evil.transform.position - player1.transform.position;

        if (compass_jammer || (dist1.magnitude < 5f || dist2.magnitude < 5f))
        {
            var ang1 = Random.Range(-180, 180);
            var ang2 = Random.Range(-180, 180);

            Quaternion quat1 = Quaternion.Euler(0f, 0f, ang1);
            Quaternion quat2 = Quaternion.Euler(0f, 0f, ang2);

            p2compass_hand.transform.rotation = Quaternion.RotateTowards(p2compass_hand.transform.rotation, quat1,
                Mathf.Abs(p2compass_hand.transform.rotation.eulerAngles.z - ang1) * Random.Range(0.5f, 4.5f) * Time.deltaTime);
            evcompass_hand.transform.rotation = Quaternion.RotateTowards(evcompass_hand.transform.rotation, quat2,
                Mathf.Abs(p2compass_hand.transform.rotation.eulerAngles.z - ang2) * Random.Range(0.5f, 4.5f) * Time.deltaTime);

            return;
        }

        var dir1 = (dist1).normalized;
        var dir2 = (dist2).normalized;

        float angle1 = Vector2.SignedAngle(Vector2.up, dir1);
        float angle2 = Vector2.SignedAngle(Vector2.up, dir2);

        Quaternion q_ang1 = Quaternion.Euler(0f, 0f, angle1);
        Quaternion q_ang2 = Quaternion.Euler(0f, 0f, angle2);

        var diff_spd1 = Mathf.Abs(p2compass_hand.transform.rotation.eulerAngles.z - angle1);
        var diff_spd2 = Mathf.Abs(evcompass_hand.transform.rotation.eulerAngles.z - angle2);

        p2compass_hand.transform.rotation = Quaternion.RotateTowards(p2compass_hand.transform.rotation, q_ang1, diff_spd1 * Time.deltaTime);
        evcompass_hand.transform.rotation = Quaternion.RotateTowards(evcompass_hand.transform.rotation, q_ang2, diff_spd2 * Time.deltaTime);
    }
}
