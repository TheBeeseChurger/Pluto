using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MazeCellScript : MonoBehaviour
{
    [field: SerializeField] public bool IsLandmarkCell { get; private set; }

    [Header("Walls")]
    [SerializeField] private GameObject l_wall;
    [SerializeField] private GameObject r_wall;
    [SerializeField] private GameObject t_wall;
    [SerializeField] private GameObject b_wall;

    [Header("Ceiling")]
    [SerializeField] private SpriteRenderer ceiling;

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
        if (!IsSeen)
        {
            if (IsLandmarkCell)
            {
                GetComponentInParent<LandmarkCellScript>().See();
            }

            IsSeen = true;
            StartCoroutine(CeilingReveal(0.5f));
        }
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

    public void PaintCell(WallColor color, float percent = 1)
    {
        switch(color)
        {
            case WallColor.red:
                Color r_color = Color.red * percent;
                r_color *= 0.5f;
                r_color.a = 1;
                l_wall.GetComponent<SpriteRenderer>().color = r_color;
                r_wall.GetComponent<SpriteRenderer>().color = r_color;
                t_wall.GetComponent<SpriteRenderer>().color = r_color;
                b_wall.GetComponent<SpriteRenderer>().color = r_color;
                break;
            case WallColor.blue:
                Color b_color = Color.blue * percent;
                b_color *= 0.5f;
                b_color.a = 1;
                l_wall.GetComponent<SpriteRenderer>().color = b_color;
                r_wall.GetComponent<SpriteRenderer>().color = b_color;
                t_wall.GetComponent<SpriteRenderer>().color = b_color;
                b_wall.GetComponent<SpriteRenderer>().color = b_color;
                break;
        }
    }

    public bool IsWallClr(WallType type)
    {
        return type switch
        {
            WallType.Left => !l_wall.activeSelf,
            WallType.Right => !r_wall.activeSelf,
            WallType.Top => !t_wall.activeSelf,
            WallType.Bottom => !b_wall.activeSelf,
            _ => false,
        };
    }

    private IEnumerator CeilingReveal(float duration)
    {
        Color color = ceiling.color;
        float time = 0;

        while (time < duration)
        {
            ceiling.color = new Color(color.r, color.g, color.b, Mathf.Lerp(1f, 0f, time / duration));

            time += Time.deltaTime;

            yield return null;
        }
        ceiling.enabled = false;
    }
}
