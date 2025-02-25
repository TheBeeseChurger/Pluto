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

    public enum WallType
    {
        Left, Right, Top, Bottom
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
}
