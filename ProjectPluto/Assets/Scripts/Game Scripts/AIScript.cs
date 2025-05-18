using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using Vec2ExtensionMethods;

namespace Vec2ExtensionMethods
{
    public static class Extensions
    {
        public static Vector2 Sub(this Vector2 left, float right)
            => new(left.x - right, left.y - right);

        public static Vector2 Add(this Vector2 left, float right)
            => new(left.x + right, left.y + right);

        public static Vector2 Round(this Vector2 left)
            => new(Mathf.Round(left.x), Mathf.Round(left.y));
    }
}

public class AIScript : MonoBehaviour
{
    public class BFSNode
    {
        public MazeCellScript cell;
        public BFSNode parent;
        public int distance;

        public BFSNode(MazeCellScript cell, int distance, BFSNode parent = null)
        {
            this.cell = cell;
            this.distance = distance;
            this.parent = parent;
        }
    }

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
            var sight_check_pos = CheckSight();

            if (sight_check_pos == last_seen_pos)
            {
                last_seen_pos = PredictPosition();
            }
            else last_seen_pos = sight_check_pos;

            //Debug.Log("Currently chasing. Last seen at: " + last_seen_pos);
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
            new_pos *= GameManager.gameTimeScale;
            transform.position = new Vector3(new_pos.x, new_pos.y, -1f);

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

        switch (my_type)
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

        if (CurrentNearestCell(last_seen_pos) == CurrentNearestCell(transform.position))
        {
            IsChasing = false;
        }

        if (!IsChasing) ChangeMode(poss_dirs);
        else run_dir = BFSPathFind(CurrentNearestCell(transform.position), CurrentNearestCell(last_seen_pos));

        if (Random.Range(0f, 1f) < 0.8f)
        {
            if (poss_dirs.Contains(last_dir) && poss_dirs.Count > 1)
            {
                poss_dirs.Remove(last_dir);
            }
        }

        switch (my_mode)
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
                    if (poss_dirs.Contains(last_dir) && poss_dirs.Count > 1)
                    {
                        poss_dirs.Remove(last_dir);
                    }

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
        foreach (var dir in dirs)
        {
            RaycastHit2D hit;
            if (my_type != AIType.evil) hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + (dir * 0.51f), dir, 5.0f);
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
        if (chasing_tag == null) return last_seen_pos;

        List<Vector2> poss_dirs = gm.PossibleDirections(transform.position);

        foreach (Vector2 dir in poss_dirs)
        {
            RaycastHit2D hit;
            if (my_type != AIType.evil) hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + (dir * 0.51f), dir, 5.0f);
            else hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + (dir * 0.71f), dir, 5.0f);

            if (!hit.collider) continue;

            if (hit.collider.CompareTag(chasing_tag))
            {
                return hit.transform.position;
            }
        }

        return last_seen_pos;
    }

    private Vector2 PredictPosition()
    {
        if (chasing_tag == null) return last_seen_pos;

        Vector3 curr_pos = GameObject.FindGameObjectWithTag(chasing_tag).transform.position;

        Vector2 last_cell = CurrentNearestCell(last_seen_pos);
        Vector2 curr_cell = CurrentNearestCell(curr_pos);

        if (last_cell != curr_cell)
        {
            chasing_tag = null;
        }

        return curr_pos;
    }

    private Vector2 CurrentNearestCell(Vector2 pos)
    {
        Vector2 res = gm.CalcMazePos(pos).Sub(0.5f);
        return res.Round();
    }
    
    private Vector2 BFSPathFind(Vector2 start, Vector2 end)
    {
        //Debug.LogError("Start: " + start + "\nEnd: " + end);
        const int _max_distance = 10;
        Queue<BFSNode> queue = new();
        HashSet<MazeCellScript> seen = new();
        Vector2 start_plus_one;

        queue.Enqueue(new BFSNode(gm.GetMazeCell(start), 0));
        seen.Add(gm.GetMazeCell(start));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            MazeCellScript current_cell = current.cell;
            int current_distance = current.distance;

            if (current_cell == gm.GetMazeCell(end))
            {
                var path = ReconstructPath(current);
                start_plus_one = CurrentNearestCell(path[1].transform.position);
                //Debug.Log("Found path...\nHeading from: " + start + " to " + start_plus_one);
                return start_plus_one - start;
            }

            if (current_distance >= _max_distance) continue;

            foreach(var cell in gm.GetMazeConnectedCell(current_cell))
            {
                if (!seen.Contains(cell))
                {
                    seen.Add(cell);
                    queue.Enqueue(new BFSNode(cell, current_distance + 1, current));
                }
            }
        }

        Debug.LogError("PATHFIND ERROR: No Path Found: " + start + " " + end);
        return Vector2.zero;
    }

    private List<MazeCellScript> ReconstructPath(BFSNode node)
    {
        List<MazeCellScript> path = new();
        while (node != null)
        {
            path.Add(node.cell);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }
}
