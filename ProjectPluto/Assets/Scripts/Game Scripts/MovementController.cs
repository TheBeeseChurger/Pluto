using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] float move_speed;

    [SerializeField] Vector2 dir;

    public bool IsMoving { get; private set; }

    private Direction curr_direction = Direction.None;

    public enum Direction
    {
        Left, Right, Up, Down, None
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeDirection(Direction new_direction = Direction.None)
    {
        if (IsMoving)
        {
            switch (new_direction)
            {
                case Direction.Left:
                    if (curr_direction == Direction.Right)
                    {
                        dir = new Vector2(-1, 0);
                        curr_direction = Direction.Left;
                    }
                    break;
                case Direction.Right:
                    if (curr_direction == Direction.Left)
                    {
                        dir = new Vector2(1, 0);
                        curr_direction = Direction.Right;
                    }
                    break;
                case Direction.Up:
                    if (curr_direction == Direction.Down)
                    {
                        dir = new Vector2(0, 1);
                        curr_direction = Direction.Up;
                    }
                    break;
                case Direction.Down:
                    if (curr_direction == Direction.Up)
                    {
                        dir = new Vector2(0, -1);
                        curr_direction = Direction.Up;
                    }
                    break;
                default:
                    break;
            }
            return;
        }
        
        switch (new_direction)
        {
            case Direction.Left:
                dir = new Vector2(-1, 0);
                curr_direction = Direction.Left;
                break;
            case Direction.Right:
                dir = new Vector2(1, 0);
                curr_direction = Direction.Right;
                break;
            case Direction.Up:
                dir = new Vector2(0, 1);
                curr_direction = Direction.Up;
                break;
            case Direction.Down:
                dir = new Vector2(0, -1);
                curr_direction = Direction.Down;
                break;
            default:
                dir = new Vector2(0, 0);
                curr_direction = Direction.None;
                break;
        }
    }
}
