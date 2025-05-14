using System.Collections;
using System.Collections.Generic;
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
            if (run_dir == Vector2.zero) new_pos = Vector2.MoveTowards(transform.position, starting_pos + move_dir, move_speed * Time.deltaTime);
            else new_pos = Vector2.MoveTowards(transform.position, starting_pos + move_dir, run_speed * Time.deltaTime);

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

        ChangeMode(poss_dirs);

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
            if(my_type != AIType.evil) hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + (dir * 0.51f), dir);
            else hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + (dir * 0.71f), dir);

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
                    return AIMode.chase;
                }

                break;
        }

        run_dir = Vector2.zero;
        return AIMode.wander;
    } 
}
