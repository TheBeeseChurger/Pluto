using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    private static int score = 110;
    private static float score_multiplier = 1f;
    public static int round = 0;

    private InputSystem_Actions _input_system;

    private SightSystem _sight_system;

    private Timer score_timer;
    private Timer distance_timer;
    private Timer degen_timer;
    private Timer call_timer;

    private int last_distance;

    private int x_pos;
    private int y_pos;

    static DataScript data;

    static GameObject audio_head;

    AudioSource song;
    AudioSource ui;

    [Header("Glitch")]
    [SerializeField] Material glitch_mat;
    List<GameObject> glitchers;

    [Header("AudioResource")]
    [SerializeField] AudioResource bgm;
    [SerializeField] AudioResource[] call;

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
    CallIndicatorScript call_indicator;
    GameObject player1;
    GameObject player2;
    GameObject evil;

    Vector2 last_evil_pos;

    RoundTriggerScript[] player2_triggers;

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

        _sight_system = new(maze_gen);

        await InstantiateAsync(_follower);
        follower = FindFirstObjectByType<CameraFollowScript>();
        follower.Init();
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

        call_indicator = FindFirstObjectByType<CallIndicatorScript>();
        call_indicator.Init(player1.transform, player2.transform);

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

        SeeStart();
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

        call_timer = gameObject.AddComponent<Timer>();

        call_timer.timer_spd = 1f;
        call_timer.timer_time = 5f;
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

    private void SeeStart()
    {
        var start_pos = CalcMazePos(player1.transform.position);

        x_pos = (int)start_pos.x;
        y_pos = (int)start_pos.y;

        _sight_system.See(maze_gen.GetCell(x_pos, y_pos));
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

            if (p1_pos == delete_pos)
            {
                EndGame(player1.transform.position);
            } 
            if (p2_pos == delete_pos)
            {
                EndGame(player2.transform.position);
            }

            if (evil != null)
            {
                temp = CalcMazePos(evil.transform.position);
                temp -= new Vector2(0.5f, 0.5f);
                (int x, int y) ev_pos = (Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y));
                
                if (ev_pos == delete_pos)
                {
                    last_evil_pos = new(ev_pos.x, ev_pos.y);
                    glitchers.Remove(evil);
                    Destroy(evil);
                }
            }

             
        }

        if (distance_timer.End && gameTimeScale != 0f)
        {
            last_distance = maze_gen.GetCellDistance(CalcMazePos(player1.transform.position), CalcMazePos(player2.transform.position));

            //Debug.Log("Updated distance to " + last_distance);

            if (last_distance < 0)
            {
                //No Path Found
                EndGame(player2.transform.position);
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

        if (call_timer.End)
        {
            if (call_timer.timer_time > 2f)
            {
                call_timer.timer_time = 2f;
                call_timer.Interrupt();

                call_indicator.TurnOn();

                ui.resource = call[Random.Range(0, call.Length)];
                ui.loop = false;
                ui.volume = 0.4f;
                ui.Play();
            }
            else
            {
                call_timer.timer_time = 10f + Random.Range(0f, 10f);
                call_timer.Interrupt();

                call_indicator.TurnOff();
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
            else if (!_sight_system.IsSeen(maze_gen.GetCell(x_pos, y_pos)))
            {
                _sight_system.See(maze_gen.GetCell(x_pos, y_pos));
            }
        }

        if (gameTimeScale == 0f) return;

        GlitchControl();
    }

    public async void EndGame(Vector2 end_pos = new Vector2())
    {
        if (new Vector2() == end_pos) 
            end_pos = player1.transform.position;

        glitch_mat.SetFloat("_chrom_aberr", 0f);
        glitch_mat.SetFloat("_chrom_aberr_offset_max", 0f);
        gameTimeScale = 0f;

        data.AddScore(score);
        data.CommitScore();
        ResetScore();

        _sight_system.AbsSee(end_pos);
        tracer.TraceStop(end_pos);

        song.Stop();
        await Awaitable.WaitForSecondsAsync(3f);

        if (round >= 25)
        {
            //Transfer to secret cutscene
        } else
        {
            ProjectManager.GameToMenu.Invoke();
        }
        //END GAME
    }

    public async Awaitable ResetGame()
    {
        tracer.CamToOrigin();

        await Awaitable.NextFrameAsync();
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

    public void FireReveal(MazeCellScript bl_cell, int radius)
    {
        Vector2Int bl_int = new Vector2Int((int)bl_cell.transform.localPosition.x, (int)bl_cell.transform.localPosition.y);

        for (int dx = 0; dx < 2; dx++)
        {
            for (int dy = 0; dy < 2; dy++)
            {
                _sight_system.OneSee(maze_gen.GetCell(bl_int.x + dx, bl_int.y + dy));
            }
        }

        for (int r = 1; r < radius; r++)
        {
            for(int x = -r - 1; x <= r + 1; x++)
            {
                for (int y = -r - 1; y <= r + 1; y++)
                {

                }
            }
        }
    }

    private int DistFromCen(Vector2Int cell, Vector2Int cen)
    {
        int minX = cen.x;
        int minY = cen.y;
        int maxX = cen.x + 1;
        int maxY = cen.y + 1;

        int clampedX = Mathf.Clamp(cell.x, minX, maxX);
        int clampedY = Mathf.Clamp(cell.y, minY, maxY);

        Vector2Int closestPoint = new Vector2Int(clampedX, clampedY);
        return (cell - closestPoint).sqrMagnitude;
    }

    private struct SightSystem
    {
        Dictionary<MazeCellScript, int> seen_maze;
        GeneratorScript gen;

        struct cellData
        {
            public int dist;
            public Vector2 dir;

            public cellData(int dist, Vector2 dir)
            {
                this.dist = dist;
                this.dir = dir;
            }
        }
        
        public SightSystem(GeneratorScript gen)
        {
            this.gen = gen;
            seen_maze = new();
        }

        public bool IsSeen(MazeCellScript location)
        {
            if (!seen_maze.ContainsKey(location)) return false;

            return seen_maze[location] == 11;
        }

        public async void See(MazeCellScript location)
        {
            //Debug.Log("Starting See Algo");

            seen_maze[location] = 11;
            Reveal(location, 11);

            Queue<KeyValuePair<MazeCellScript, cellData>> queue = new();

            queue.Enqueue(new KeyValuePair<MazeCellScript, cellData>(location, new cellData(0, Vector2.zero)));

            while (queue.Count > 0)
            {
                if (gameTimeScale <= 0) return;

                //Debug.Log("New Cell");
                var current = queue.Dequeue();
                MazeCellScript curr_cell = current.Key;
                cellData curr_data = current.Value;
                int curr_v_level = seen_maze[curr_cell];

                if (curr_v_level == 11)
                {
                    //Debug.Log("A Cell has perfect vision.");
                    var next_cells = gen.GetNeighboringCells(curr_cell);

                    foreach (var cell in next_cells)
                    {
                        if (cell == null) continue;

                        if (seen_maze.ContainsKey(cell))
                            if (seen_maze[cell] == 11) continue;

                        if (gen.HasWalls(curr_cell, cell))
                        {
                            if (!seen_maze.ContainsKey(cell)) seen_maze[cell] = 2;
                            else if (seen_maze[cell] < 2) seen_maze[cell] = 2;
                            else continue;
                        }
                        else
                        {
                            if (!seen_maze.ContainsKey(cell)) seen_maze[cell] = curr_v_level - 1;
                            else if (seen_maze[cell] < curr_v_level - 1) seen_maze[cell] = curr_v_level - 1;
                            else continue;
                        }

                        cellData data = new(curr_data.dist++, GetCellDir(curr_cell, cell));
                        Reveal(cell, seen_maze[cell]);
                        queue.Enqueue(new KeyValuePair<MazeCellScript, cellData>(cell, data));
                    }
                }
                else if (curr_v_level > 1)
                {
                    var next_cells = gen.GetConnectedCells(curr_cell);

                    foreach (var cell in next_cells)
                    {
                        if (cell == null) continue;

                        if (GetCellDir(curr_cell, cell) == curr_data.dir)
                            if (!seen_maze.ContainsKey(cell)) seen_maze[cell] = curr_v_level - 1;
                            else if (seen_maze[cell] < curr_v_level - 1) seen_maze[cell] = curr_v_level - 1;
                            else continue;
                        else
                            if (!seen_maze.ContainsKey(cell)) seen_maze[cell] = curr_v_level - 2;
                            else if (seen_maze[cell] < curr_v_level - 2) seen_maze[cell] = curr_v_level - 2;
                            else continue;

                        cellData data = new(curr_data.dist++, GetCellDir(curr_cell, cell));
                        Reveal(cell, seen_maze[cell]);
                        queue.Enqueue(new KeyValuePair<MazeCellScript, cellData>(cell, data));
                    }
                }
                await Awaitable.NextFrameAsync();
            }
        }

        public void OneSee(MazeCellScript location)
        {
            if (location == null) return;
            if (seen_maze[location] == 11) return;
            seen_maze[location] = 11;
            Reveal(location, seen_maze[location]);
        }

        public void AbsSee(Vector2 see_pos)
        {
            Vector2Int vec = new Vector2Int(Mathf.RoundToInt(see_pos.x - gen.transform.position.x + 0.5f), Mathf.RoundToInt(see_pos.y - gen.transform.position.y + 0.5f));

            for (int i = -10; i <= 10; i++)
            {
                for (int j = -10; j <= 10; j++)
                {
                    if (!gen.CheckCell(vec.x + i, vec.y + j)) continue;
                    var cell = gen.GetCell(vec.x + i, vec.y + j);

                    if (cell == null) continue;
                    Reveal(cell, 11, false);
                }
            }
        }

        private void Reveal(MazeCellScript location, int vision_level, bool score = true)
        {
            //Debug.Log("Vision level of cell: " + vision_level);
            if (location.IsLandmarkCell) location.GetComponentInParent<LandmarkCellScript>().See(VisionToFloat(vision_level));
            else location.See(VisionToFloat(vision_level));

            if (vision_level > 8 && score) Score(location);
        }

        private void Score(MazeCellScript location)
        {
            if (location.IsLandmarkCell)
            {
                score += (int)(100 * score_multiplier);
            }
            else
            {
                score += (int)(10 * score_multiplier);
            }
        }

        private float VisionToFloat(int vision_level)
        {
            if (vision_level > 7)
                return 0.0f;
            else if (vision_level > 5)
                return 0.25f;
            else if (vision_level > 3)
                return 0.5f;
            else if (vision_level > 2)
                return 0.75f;
            else 
                return 1.0f;
        }

        private Vector2 GetCellDir(MazeCellScript prev_cell, MazeCellScript next_cell)
        {
            var type = gen.GetDir(prev_cell, next_cell);

            switch (type)
            {
                case MazeCellScript.WallType.Left:
                    return Vector2.left;
                case MazeCellScript.WallType.Right:
                    return Vector2.right;
                case MazeCellScript.WallType.Top:
                    return Vector2.up;
                case MazeCellScript.WallType.Bottom:
                    return Vector2.down;
                default:
                    return Vector2.zero;
            }
        }
    };
}
