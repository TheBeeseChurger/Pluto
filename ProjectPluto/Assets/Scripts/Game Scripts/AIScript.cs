using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    private int horizontal;
    private int vertical;

    [SerializeField] private float move_speed;

    [SerializeField] GameManager gm;

    [SerializeField] private AIType type;
    [SerializeField] private AIMode mode;

    private Vector2 run_dir;
    private Vector2 move_dir;
    private Vector2 curr_pos;
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

    void Update()
    {
        if (!IsMoving)
        {
            IsMoving = true;
            
            curr_pos = transform.position;
            PickNewDirection();
        }
    }

    private void FixedUpdate()
    {
        if (IsMoving)
        {
            var new_pos = Vector2.MoveTowards(transform.position, curr_pos + move_dir, move_speed * Time.deltaTime);

            transform.position = new Vector3(new_pos.x, new_pos.y, -1f);

            if ((Vector2)transform.position == (curr_pos + move_dir))
            {
                last_dir = move_dir;
                last_dir.Scale(new Vector2(-1f, -1f));

                IsMoving = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(type)
        {
            case AIType.p1:
                if (collision.collider.CompareTag("Player2"))
                {
                    gm.NextRound();
                }

                if (collision.collider.CompareTag("Evil"))
                {
                    gm.EndGame();
                }

                break;
            case AIType.p2:
                if (collision.collider.CompareTag("Player"))
                {
                    gm.NextRound();
                }

                if (collision.collider.CompareTag("Evil"))
                {
                    gm.EndGame();
                }

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
        

        switch(mode)
        {
            case AIMode.wander:
                var chosen_dir = poss_dirs[Random.Range(0, poss_dirs.Count)];

                horizontal = (int)chosen_dir.x;
                vertical = (int)chosen_dir.y;

                break;
            case AIMode.chase:
                horizontal = (int)run_dir.x;
                vertical = (int)run_dir.y;

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
        foreach(var dir in dirs)
        {
            var hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + (dir * 0.51f), dir);

            if (!hit)
            {
                mode = AIMode.wander;
                continue;
            }
            
            if (!hit.collider.CompareTag("Untagged"))
                mode = CheckHit(hit, dir);
        }
    }

    private AIMode CheckHit(RaycastHit2D hit, Vector2 dir)
    {
        switch (type)
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
                    //Debug.Log("Chasing");
                    return AIMode.chase;
                }

                break;
        }

        run_dir = Vector2.zero;
        return AIMode.wander;
    } 
}
