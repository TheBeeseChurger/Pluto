using System;
using System.Collections;
using UnityEngine;

public class LandmarkCellScript : MonoBehaviour
{
    [SerializeField] MazeCellScript cell_bl;
    [SerializeField] MazeCellScript cell_br;
    [SerializeField] MazeCellScript cell_tl;
    [SerializeField] MazeCellScript cell_tr;

    [SerializeField] SpriteRenderer ceiling;
    [SerializeField] SpriteRenderer floor;

    [SerializeField] Sprite[] cracked;

    public float curr_alpha = 1f;

    private GeneratorScript maze;

    private int maze_x;
    private int maze_y;

    public bool IsSeen { get; private set; }

    public void See(float percent_fade)
    {
        if (IsSeen) return;

        if (percent_fade <= 0) IsSeen = true;

        curr_alpha = percent_fade;

        StopAllCoroutines();
        StartCoroutine(CeilingReveal(0.6f, percent_fade));

        cell_bl.See(percent_fade);
        cell_br.See(percent_fade);
        cell_tl.See(percent_fade);
        cell_tr.See(percent_fade);
    }

    public async void Delete()
    {
        int rand = UnityEngine.Random.Range(0, cracked.Length);

        floor.sprite = cracked[rand];
        ceiling.sprite = cracked[rand];

        try
        {
            await Awaitable.WaitForSecondsAsync(2f, destroyCancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        _ = cell_bl.Delete(maze_x, maze_y);
        _ = cell_br.Delete(maze_x + 1, maze_y);
        _ = cell_tl.Delete(maze_x + 1, maze_y + 1);
        _ = cell_tr.Delete(maze_x, maze_y + 1);

        Destroy(gameObject);
    }

    public MazeCellScript GetCell(int x, int y)
    {
        if (x == 1)
        {
            if (y == 1)
            {
                return cell_bl;
            }
            else if (y == 2)
            {
                return cell_tl;
            }
        }
        else if (x == 2)
        {
            if (y == 1)
            {
                return cell_br;
            }
            else if (y == 2)
            {
                return cell_tr;
            }
        }

        return null;
    }

    public (int x, int y) GetXY()
    {
        return (maze_x, maze_y);
    }

    public void Initialize(GeneratorScript new_maze)
    {
        maze = new_maze;

        maze_x = (int)transform.localPosition.x;
        maze_y = (int)transform.localPosition.y;

        CopyCells();
    }

    private void CopyCells()
    {
        var cell1 = maze.GetCell(maze_x, maze_y);
        var cell2 = maze.GetCell(maze_x + 1, maze_y);
        var cell3 = maze.GetCell(maze_x + 1, maze_y + 1);
        var cell4 = maze.GetCell(maze_x, maze_y + 1);

        CopyCell(cell1, cell_bl);
        CopyCell(cell2, cell_br);
        CopyCell(cell3, cell_tr);
        CopyCell(cell4, cell_tl);

        maze.OverrideCell(cell_bl, maze_x, maze_y);
        maze.OverrideCell(cell_br, maze_x + 1, maze_y);
        maze.OverrideCell(cell_tr, maze_x + 1, maze_y + 1);
        maze.OverrideCell(cell_tl, maze_x, maze_y + 1);
    }

    private void CopyCell(MazeCellScript copycell, MazeCellScript clonecell)
    {
        if (copycell.IsWallClr(MazeCellScript.WallType.Left))
        {
            clonecell.ClrWall(MazeCellScript.WallType.Left);
        }

        if (copycell.IsWallClr(MazeCellScript.WallType.Right))
        {
            clonecell.ClrWall(MazeCellScript.WallType.Right);
        }

        if (copycell.IsWallClr(MazeCellScript.WallType.Top))
        {
            clonecell.ClrWall(MazeCellScript.WallType.Top);
        }

        if (copycell.IsWallClr(MazeCellScript.WallType.Bottom))
        {
            clonecell.ClrWall(MazeCellScript.WallType.Bottom);
        }

        if (copycell.IsLocked)
        {
            clonecell.Lock();
        }

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
