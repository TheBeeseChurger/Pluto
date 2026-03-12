using System;
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
    [SerializeField] private Sprite[] ceiling_cracked;

    [Header("Floor")]
    [SerializeField] private SpriteRenderer floor;
    [SerializeField] private Sprite[] floor_cracked;

    public event Action<MazeCellScript, int, int> OnDeleted;

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

    public void See(float percent_fade)
    {
        if (!IsSeen)
        {
            if (IsLandmarkCell)
            {
                if (percent_fade <= 0)
                    IsSeen = true;

                return;
            }

            StopAllCoroutines();
            StartCoroutine(CeilingReveal(0.6f, percent_fade));
            if (percent_fade > 0.0f ) return;
            IsSeen = true;
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

    public void InvisWall(WallType type)
    {
        switch(type)
        {
            case WallType.Left:
                l_wall.SetActive(true);
                l_wall.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
                l_wall.GetComponent<BoxCollider2D>().enabled = false;
                break;
            case WallType.Right:
                r_wall.SetActive(true);
                r_wall.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
                r_wall.GetComponent<BoxCollider2D>().enabled = false;
                break;
            case WallType.Top:
                t_wall.SetActive(true);
                t_wall.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
                t_wall.GetComponent<BoxCollider2D>().enabled = false;
                break;
            case WallType.Bottom:
                b_wall.SetActive(true);
                b_wall.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
                b_wall.GetComponent<BoxCollider2D>().enabled = false;
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
                r_color.a = 1;
                l_wall.GetComponent<SpriteRenderer>().color = r_color;
                r_wall.GetComponent<SpriteRenderer>().color = r_color;
                t_wall.GetComponent<SpriteRenderer>().color = r_color;
                b_wall.GetComponent<SpriteRenderer>().color = r_color;
                break;
            case WallColor.blue:
                Color b_color = Color.blue * percent;
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

    public async Awaitable Delete(int x, int y)
    {
        if (IsLandmarkCell)
        {
            OnDeleted?.Invoke(this, x, y);
            return;
        }

        int rand = UnityEngine.Random.Range(0, floor_cracked.Length);

        floor.sprite = floor_cracked[rand];
        ceiling.sprite = ceiling_cracked[rand];

        try
        {
            await Awaitable.WaitForSecondsAsync(2f, destroyCancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        OnDeleted?.Invoke(this, x, y);
        Destroy(gameObject);
    }

    private IEnumerator CeilingReveal(float duration, float end_amt)
    {
        Color color = ceiling.color;
        float time = 0f;
        float start_amt = color.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            
            color.a = Mathf.Lerp(start_amt, end_amt, t);
            ceiling.color = color;

            yield return null;
        }

        color.a = end_amt;
        ceiling.color = color;

        if (Mathf.Approximately(color.a, 0.0f)) ceiling.enabled = false;
    }
}
