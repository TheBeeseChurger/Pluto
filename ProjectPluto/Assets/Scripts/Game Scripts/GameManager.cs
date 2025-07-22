using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    private static int score = 110;
    private static float score_multiplier = 1f;
    public static int round = 0;

    private InputSystem_Actions _input_system;

    private Timer score_timer;
    private Timer distance_timer;
    private Timer degen_timer;

    private int last_distance;

    private int x_pos;
    private int y_pos;

    private bool compass_jammer = false;

    static DataScript data;

    static GameObject audio_head;

    AudioSource song;
    AudioSource ui;

    [Header("Glitch")]
    [SerializeField] Material glitch_mat;
    List<GameObject> glitchers;

    [Header("AudioResource")]
    [SerializeField] AudioResource bgm;

    [Header("Initialization References")]
    [SerializeField] GameObject _maze;
    [SerializeField] GameObject _tracer;
    [SerializeField] GameObject _follower;
    [SerializeField] GameObject _p1;
    [SerializeField] GameObject _p2;
    [SerializeField] GameObject _evil;

    GeneratorScript maze_gen;
    CameraTraceScript tracer;
    CameraFollowScript follower;
    GameObject player1;
    GameObject player2;
    GameObject evil;

    RoundTriggerScript[] player2_triggers;

    GameObject p2compass_hand;
    GameObject evcompass_hand;

    float pro_compass_rand_dir = 0f;
    float evil_compass_rand_dir = 0f;

    TextMeshProUGUI score_text;
    TextMeshProUGUI multiplier_text;
    TextMeshProUGUI round_text;

    private bool IsInitalizing = true;

    public static float gameTimeScale = 1f;

    public async Awaitable GamePreStart(DataScript new_data, GameObject new_audio)
    {
        if (new_audio != null)
        {
            audio_head = new_audio;
        }

        if (new_data != null)
        {
            data = new_data;
        }

        Init();

        //Instantiate & Init
        await Awaitable.MainThreadAsync();
        await InstantiateAsync(_maze);
        maze_gen = FindFirstObjectByType<GeneratorScript>();

        await InstantiateAsync(_follower);
        follower = FindFirstObjectByType<CameraFollowScript>();
        follower.Init();
        p2compass_hand = GameObject.Find("ProserpinaHand");
        evcompass_hand = GameObject.Find("CeresHand");
        score_text = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        multiplier_text = GameObject.Find("MultiplierText").GetComponent<TextMeshProUGUI>();
        round_text = GameObject.Find("RoundText").GetComponent<TextMeshProUGUI>();

        await InstantiateAsync(_p1);
        player1 = GameObject.FindGameObjectWithTag("Player");
        player1.GetComponent<PlayerScript>().Init(this);

        await InstantiateAsync(_tracer);
        tracer = FindFirstObjectByType<CameraTraceScript>();
        tracer.Init();

        await InstantiateAsync(_p2);
        player2 = GameObject.FindGameObjectWithTag("Player2");
        player2.GetComponent<AIScript>().Init(this);
        player2_triggers = FindObjectsByType<RoundTriggerScript>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var trigger in player2_triggers)
        {
            trigger.Init(this);
        }

        await InstantiateAsync(_evil);
        evil = GameObject.FindGameObjectWithTag("Evil");
        evil.GetComponent<AIScript>().Init(this);

        if (song.resource != bgm)
        {
            song.resource = bgm;
            song.loop = true;
            song.volume = 0.45f;
            song.Play();
        }

        MazeInit();
        SeeInit();
        TimerInit();
        GlitchInit();

        tracer.CamReset();

        round++;
        round_text.text = "Round:" + round;
    }

    public void GameStart()
    {
        tracer.TraceStart();

        player1.GetComponent<PlayerScript>().PlayerStart();
        player2.GetComponent<AIScript>().AIStart();
        evil.GetComponent<AIScript>().AIStart();

        distance_timer.Interrupt();
        score_timer.Interrupt();
        degen_timer.Interrupt();

        IsInitalizing = false;
        gameTimeScale = 1f;
        glitch_mat.SetFloat("_chrom_aberr", 1f);
    }

    private void Init()
    {
        song = audio_head.transform.GetChild(0).GetComponent<AudioSource>();
        ui = audio_head.transform.GetChild(2).GetComponent<AudioSource>();

        _input_system = new();
    }

    private void TimerInit()
    {
        score_timer = gameObject.AddComponent<Timer>();

        score_timer.timer_spd = 1f;
        score_timer.timer_time = 1f;

        distance_timer = gameObject.AddComponent<Timer>();

        distance_timer.timer_spd = 1f;
        distance_timer.timer_time = 5f;

        degen_timer = gameObject.AddComponent<Timer>();

        degen_timer.timer_spd = 1f;
        degen_timer.timer_time = 1f;
    }

    private void MazeInit()
    {
        maze_gen.Generate();

        var p1_cell = maze_gen.GetPlayer1Spawn();
        var p2_cell = maze_gen.GetPlayer2Spawn();
        var evil_cell = maze_gen.GetEvilSpawn();

        player1.transform.position = new Vector3(p1_cell.transform.position.x, p1_cell.transform.position.y, player1.transform.position.z);
        player2.transform.position = new Vector3(p2_cell.transform.position.x, p2_cell.transform.position.y, player2.transform.position.z);
        evil.transform.position = new Vector3(evil_cell.transform.position.x, evil_cell.transform.position.y, evil.transform.position.z);

        last_distance = maze_gen.GetCellDistance(p1_cell, p2_cell);
    }

    private void SeeInit()
    {
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
    }

    private void GlitchInit()
    {
        glitchers = new List<GameObject>()
        {
            evil
        };
    }

    void Update()
    {
        if (score_text != null) score_text.text = "Score:" + score;
        if (multiplier_text != null) multiplier_text.text = "Multipler x" + score_multiplier;

        if (IsInitalizing) return;

        if (score_timer.End && score >= 10)
        {
            score -= 10;
        }

        if (degen_timer.End)
        {
            (int x, int y) delete_pos = maze_gen.DeleteRandomCell();
            var temp = CalcMazePos(player1.transform.position);
            //temp -= new Vector2 (0.5f, 0.5f);
            (int x, int y) p1_pos = (Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y));
            temp = CalcMazePos(player2.transform.position);
            temp -= new Vector2(0.5f, 0.5f);
            (int x, int y) p2_pos = (Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y));
            temp = CalcMazePos(evil.transform.position);
            temp -= new Vector2(0.5f, 0.5f);
            (int x, int y) ev_pos = (Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y));

            if (p1_pos == delete_pos || p2_pos == delete_pos)
            {
                EndGame();
            } else if (ev_pos == delete_pos)
            {
                Destroy(evil);
            }
        }

        if (distance_timer.End && gameTimeScale != 0f)
        {
            last_distance = maze_gen.GetCellDistance(CalcMazePos(player1.transform.position), CalcMazePos(player2.transform.position));

            //Debug.Log("Updated distance to " + last_distance);

            if (last_distance < 0)
            {
                EndGame();
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

        var curr_pos = CalcMazePos(player1.transform.position);

        if (x_pos != (int)curr_pos.x || y_pos != (int)curr_pos.y)
        {
            x_pos = (int)curr_pos.x;
            y_pos = (int)curr_pos.y;

            if (maze_gen.GetCell(x_pos, y_pos) == null)
            {
                EndGame();
                return;
            }

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

        if (gameTimeScale == 0f) return;

        CompassControl();
        GlitchControl();
    }

    public void EndGame()
    {
        glitch_mat.SetFloat("_chrom_aberr", 0f);
        glitch_mat.SetFloat("_chrom_aberr_offset_max", 0f);
        gameTimeScale = 0f;

        data.AddScore(score);
        data.CommitScore();
        ResetScore();

        tracer.TraceStop();

        ProjectManager.GameToMenu.Invoke();
        //END GAME
    }

    public void NextRound()
    {
        glitch_mat.SetFloat("_chrom_aberr", 0f);
        glitch_mat.SetFloat("_chrom_aberr_offset_max", 0f);
        gameTimeScale = 0f;

        score += (int)(500 * score_multiplier);
        score_multiplier += 0.5f;

        ProjectManager.GameToGame.Invoke();
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

    public Vector2 CalcMazePos(Vector3 pos)
    {
        return CalcMazePos(pos.x, pos.y);
    }

    public Vector2 CalcMazePos(Vector2 pos)
    {
        return CalcMazePos(pos.x, pos.y);
    }

    public Vector2 CalcMazePos(float x, float y)
    {
        return new Vector2(x - maze_gen.transform.position.x + 0.5f, y - maze_gen.transform.position.y + 0.5f);
    }

    public MazeCellScript GetMazeCell(Vector2 pos)
    {
        return maze_gen.GetCell((int)pos.x, (int)pos.y);
    }

    public IEnumerable<MazeCellScript> GetMazeConnectedCell(MazeCellScript cell)
    {
        return maze_gen.GetConnectedCells(cell);
    }

    private void ResetScore()
    {
        score = 110;
        score_multiplier = 1;
        round = 0;
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
            if (pro_compass_rand_dir == 0f)
            {
                pro_compass_rand_dir = 300f * Random.Range(0.5f, 1.5f);
            }

            if (evil_compass_rand_dir == 0)
            {
                evil_compass_rand_dir = 300f * Random.Range(0.5f, 1.5f);
            }

            Quaternion quat1 = Quaternion.Euler(0f, 0f, pro_compass_rand_dir * Time.deltaTime);
            Quaternion quat2 = Quaternion.Euler(0f, 0f, evil_compass_rand_dir * Time.deltaTime);

            p2compass_hand.transform.rotation *= quat1;
            evcompass_hand.transform.rotation *= quat2;

            return;
        }

        pro_compass_rand_dir = 0f;
        evil_compass_rand_dir = 0f;

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

    private void GlitchControl()
    {
        float val = 0f;
        foreach (GameObject glitch in glitchers)
        {
            var g_dist = (glitch.transform.position - player1.transform.position).magnitude;

            if (g_dist > 5f) continue;

            float m_val = (1f / 8f) * Mathf.Pow(g_dist - 5f, 2f) * 0.0105f;
            if (val < m_val)
                val = m_val;
        }
        glitch_mat.SetFloat("_chrom_aberr_offset_max", val);
    }

    private struct SightSystem
    {
        Dictionary<MazeCellScript, int> seen_maze;
        GeneratorScript gen;
        
        private void See(MazeCellScript location)
        {
            if (seen_maze.ContainsKey(location))
            {
                if (seen_maze[location] == 11)
                    return;
                seen_maze[location] = 11;
            } else
            {
                seen_maze.Add(location, 11);
            }
                

            
        }

        private void Reveal(MazeCellScript location, int vision_level)
        {

        }

        private float VisionToFloat(int vision_level)
        {
            if (vision_level > 11)
                return -1f;
            if (vision_level > 9)
                if (vision_level > 7)
                    if (vision_level > 5)
                        if (vision_level > 3)

        }
    };
}
