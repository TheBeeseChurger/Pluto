using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    private int horizontal;
    private int vertical;

    [SerializeField] private float move_speed;
    [SerializeField] private float run_speed;

    GameManager gm;

    [SerializeField] private AIType my_type;
    [SerializeField] private AIMode my_mode;

    private Vector2 run_dir;
    private Vector2 move_dir;
    private Vector2 starting_pos;
    private Vector2 last_dir;

    private bool IsMoving;
    private bool IsChasing;

    private Vector2 last_seen_pos;

    private string chasing_tag;

    public enum AIType
    {
        p1,
        p2,
        evil
    }

    public enum AIMode
    {
        wander,
        chase,
        flee
    }
    bool IsInitializing = true;

    public void Init(GameManager new_gm)
    {
        if (new_gm != null)
        {
            gm = new_gm;
        }
    }

    public void AIStart()
    {
        IsInitializing = false;
    }

    void Update()
    {
        if (IsInitializing) return;

        if (IsChasing)
        {
            last_seen_pos = CheckSight();
            Debug.Log("Currently chasing. Last seen at: " +  last_seen_pos);
        }
        
        if (last_seen_pos != Vector2.zero) Debug.DrawLine(transform.position, last_seen_pos);

        if (!IsMoving)
        {
            IsMoving = true;
            
            starting_pos = transform.position;
            PickNewDirection();
        }
    }

    private void FixedUpdate()
    {
        if (IsInitializing) return;

        if (IsMoving)
        {
            Vector2 new_pos;
            if (run_dir == Vector2.zero) new_pos = Vector2.MoveTowards(transform.position, starting_pos + move_dir, move_speed * Time.fixedDeltaTime);
            else new_pos = Vector2.MoveTowards(transform.position, starting_pos + move_dir, run_speed * Time.fixedDeltaTime);

            transform.position = new Vector3((new_pos.x) * GameManager.gameTimeScale, (new_pos.y) * GameManager.gameTimeScale, -1f);

            Debug.DrawLine(transform.position, starting_pos + move_dir);

            if ((Vector2)transform.position == (starting_pos + move_dir))
            {
                last_dir = move_dir;
                last_dir.Scale(new Vector2(-1f, -1f));

                IsMoving = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsInitializing || GameManager.gameTimeScale == 0f) return;

        switch(my_type)
        {
            case AIType.p1:
            case AIType.p2:
                break;
            case AIType.evil:
                if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Player2"))
                {
                    gm.EndGame();
                }

                break;
        }
    }

    private void PickNewDirection()
    {
        List<Vector2> poss_dirs = gm.PossibleDirections(transform.position);

        if (!IsChasing) ChangeMode(poss_dirs);
        else run_dir = BFSPathFind(CurrentNearestCell(transform.position), CurrentNearestCell(last_seen_pos));

        if (Random.Range(0f, 1f) < 0.8f)
        {
            if (poss_dirs.Contains(last_dir) && poss_dirs.Count > 1)
            {
                poss_dirs.Remove(last_dir);
            }
        }

        switch(my_mode)
        {
            case AIMode.wander:
                var chosen_dir = poss_dirs[Random.Range(0, poss_dirs.Count)];

                horizontal = (int)chosen_dir.x;
                vertical = (int)chosen_dir.y;

                break;
            case AIMode.chase:
                if (poss_dirs.Contains(run_dir))
                {
                    horizontal = (int)run_dir.x;
                    vertical = (int)run_dir.y;
                }
                else
                {
                    var rand_dir = poss_dirs[Random.Range(0, poss_dirs.Count)];

                    horizontal = (int)rand_dir.x;
                    vertical = (int)rand_dir.y;
                }

                break;
            case AIMode.flee:
                if (poss_dirs.Contains(run_dir))
                {
                    horizontal = (int)run_dir.x;
                    vertical = (int)run_dir.y;
                }
                else
                {
                    var rand_dir = poss_dirs[Random.Range(0, poss_dirs.Count)];

                    horizontal = (int)rand_dir.x;
                    vertical = (int)rand_dir.y;
                }

                break;
        }

        move_dir = new Vector2(horizontal, vertical);
    }

    private void ChangeMode(IEnumerable<Vector2> dirs)
    {
        bool found_hit = false;
        foreach(var dir in dirs)
        {
            RaycastHit2D hit;
            if(my_type != AIType.evil) hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + (dir * 0.51f), dir, 5.0f);
            else hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + (dir * 0.71f), dir, 5.0f);

            if (!hit.collider) continue;
            
            if (!hit.collider.CompareTag("Wall"))
            {
                my_mode = CheckHit(hit, dir);
                found_hit = true;
                continue;
            }
        }

        if (!found_hit)
        {
            my_mode = AIMode.wander;
            run_dir = Vector2.zero;
        }
    }

    private AIMode CheckHit(RaycastHit2D hit, Vector2 dir)
    {
        switch (my_type)
        {
            case AIType.p1:
                if (hit.collider.CompareTag("Player2"))
                {
                    run_dir = dir;
                    IsChasing = true;
                    chasing_tag = hit.collider.tag;
                    return AIMode.chase;
                }

                if (hit.collider.CompareTag("Evil"))
                {
                    run_dir = -dir;
                    return AIMode.flee;
                }

                break;
            case AIType.p2:
                if (hit.collider.CompareTag("Player"))
                {
                    run_dir = dir;
                    IsChasing = true;
                    chasing_tag = hit.collider.tag;
                    return AIMode.chase;
                }

                if (hit.collider.CompareTag("Evil"))
                {
                    run_dir = -dir;
                    return AIMode.flee;
                }

                break;
            case AIType.evil:
                if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Player2"))
                {
                    run_dir = dir;
                    IsChasing = true;
                    chasing_tag = hit.collider.tag;
                    return AIMode.chase;
                }

                break;
        }

        run_dir = Vector2.zero;
        return AIMode.wander;
    }

    private Vector2 CheckSight()
    {
        List<Vector2> poss_dirs = gm.PossibleDirections(transform.position);

        foreach (Vector2 dir in poss_dirs)
        {
            RaycastHit2D hit;
            if (my_type != AIType.evil) hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + (dir * 0.51f), dir, 5.0f);
            else hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + (dir * 0.71f), dir, 5.0f);

            if (!hit.collider) continue;

            if (hit.collider.CompareTag(chasing_tag))
            {
                run_dir = dir;
                return hit.transform.position;
            }
        }

        Vector2 curr_maze_pos = CurrentNearestCell(GameObject.FindGameObjectWithTag(chasing_tag).transform.position);
        Vector2 last_maze_pos = CurrentNearestCell(last_seen_pos);

        if (last_maze_pos != curr_maze_pos)
        {
            IsChasing = false;
            chasing_tag = null;
        }
        return curr_maze_pos;
    }

    private Vector2 CurrentNearestCell(Vector2 pos)
    {
        Vector2 res = gm.CalcMazePos(pos);
        return new Vector2(Mathf.Round(res.x), Mathf.Round(res.y));
    }
    private Vector2 BFSPathFind(Vector2 start, Vector2 end)
    {
        const int _max_distance = 10;
        Queue<KeyValuePair<MazeCellScript, int>> queue = new();
        bool[,] seen = new bool[21, 21];
        Vector2 seen_offset = new(start.x - 10.0f, start.y - 10.0f);
        Vector2 start_plus_one = start;

        queue.Enqueue(new KeyValuePair<MazeCellScript, int>(gm.GetMazeCell(start), 0));
        seen[10, 10] = true;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            MazeCellScript current_cell = current.Key;
            int current_distance = current.Value;

            if (current_distance == 1)
            {
                start_plus_one = gm.CalcMazePos(current_cell.transform.position);
            }

            if (current_cell == gm.GetMazeCell(end))
            {
                break;
            }

            if (current_distance >= _max_distance) continue;

            var cells = gm.GetMazeConnectedCell(current_cell);

            foreach(var cell in cells)
            {
                var pos = gm.CalcMazePos(cell.transform.position);

                var seen_pos = (pos - seen_offset);

                if (!seen[(int)seen_pos.x, (int)seen_pos.y])
                {
                    seen[(int)seen_pos.x, (int)seen_pos.y] = true;
                    queue.Enqueue(new KeyValuePair<MazeCellScript, int>(cell, current_distance + 1));
                }
            }
        }

        if (queue.Count == 0) Debug.LogError("PATHFIND ERROR: No Path Found");
        else
        {
            return (start - start_plus_one);
        }

        return Vector2.zero;
    }
}
