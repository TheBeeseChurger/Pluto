using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            PickNewDirection();

            curr_pos = transform.position;

            IsMoving = true;
        }
    }

    private void FixedUpdate()
    {
        if (IsMoving)
        {
            transform.position = Vector2.Lerp(transform.position, curr_pos + move_dir, move_speed);

            if ((Vector2)transform.position == (curr_pos + move_dir))
            {
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
        var poss_dirs = gm.PossibleDirections(transform.position);

        ChangeMode(poss_dirs);

        switch(mode)
        {
            case AIMode.wander:
                var chosen_dir = poss_dirs.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();

                horizontal = (int)chosen_dir.x;
                vertical = (int)chosen_dir.y;

                break;
            case AIMode.chase:
                horizontal = (int)run_dir.x;
                vertical = (int)run_dir.y;

                break;
            case AIMode.flee:
                horizontal = (int)run_dir.x;
                vertical = (int)run_dir.y;

                break;
        }

        move_dir = new Vector2(horizontal, vertical);
    }

    private void ChangeMode(IEnumerable<Vector2> dirs)
    {
        foreach(var dir in dirs)
        {
            var hit = Physics2D.Raycast(transform.position, dir);

            if (!hit)
                continue;
            
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
                    return AIMode.chase;
                }

                break;
        }

        run_dir = Vector2.zero;
        return AIMode.wander;
    } 
}
