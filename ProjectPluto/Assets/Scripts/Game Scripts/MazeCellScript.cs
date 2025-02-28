using UnityEngine;

public class MazeCellScript : MonoBehaviour
{
    [Header("Walls")]
    [SerializeField] private GameObject l_wall;
    [SerializeField] private GameObject r_wall;
    [SerializeField] private GameObject t_wall;
    [SerializeField] private GameObject b_wall;

    [Header("Ceiling")]
    [SerializeField] private GameObject ceiling;

    public bool IsSeen { get; private set; }
    public bool IsVisited { get; private set; }
    public bool IsLocked { get; private set; }

    public enum WallType
    {
        Left, Right, Top, Bottom
    }

    public enum WallColor
    {
        red,
        blue
    }

    public void See()
    {
        IsSeen = true;
        ceiling.SetActive(false);
    }

    public void Visit()
    {
        IsVisited = true;
    }

    public void Lock()
    {
        IsLocked = true;
    }

    public void ClrWall(WallType type)
    {
        switch(type)
        {
            case WallType.Left:
                l_wall.SetActive(false);
                break;
            case WallType.Right:
                r_wall.SetActive(false);
                break;
            case WallType.Top:
                t_wall.SetActive(false);
                break;
            case WallType.Bottom:
                b_wall.SetActive(false);
                break;
        }
    }

    public void PaintWall(WallType type, WallColor color)
    {
        switch(color)
        {
            case WallColor.red:
                switch (type)
                {
                    case WallType.Left:
                        l_wall.GetComponent<SpriteRenderer>().color = Color.red;
                        break;
                    case WallType.Right:
                        r_wall.GetComponent<SpriteRenderer>().color = Color.red;
                        break;
                    case WallType.Top:
                        t_wall.GetComponent<SpriteRenderer>().color = Color.red;
                        break;
                    case WallType.Bottom:
                        b_wall.GetComponent<SpriteRenderer>().color = Color.red;
                        break;
                }
                break;
            case WallColor.blue:
                switch (type)
                {
                    case WallType.Left:
                        l_wall.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                    case WallType.Right:
                        r_wall.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                    case WallType.Top:
                        t_wall.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                    case WallType.Bottom:
                        b_wall.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                }
                break;
        }
        
    }
}
