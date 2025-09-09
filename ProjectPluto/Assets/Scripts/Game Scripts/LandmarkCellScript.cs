using System.Collections;
using UnityEngine;

public class LandmarkCellScript : MonoBehaviour
{
    [SerializeField] MazeCellScript cell_bl;
    [SerializeField] MazeCellScript cell_br;
    [SerializeField] MazeCellScript cell_tl;
    [SerializeField] MazeCellScript cell_tr;

    [SerializeField] SpriteRenderer ceiling;
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
