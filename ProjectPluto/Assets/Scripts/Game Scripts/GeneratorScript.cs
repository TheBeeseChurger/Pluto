using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    [SerializeField] private MazeCellScript cell;

    [SerializeField] private int cell_width;
    [SerializeField] private int cell_length;

    [SerializeField] private MazeCellScript[,] maze_grid;

    private void Start()
    {
        maze_grid = new MazeCellScript[cell_width, cell_length];

        transform.position = new Vector3((cell_width / -2f) + 0.5f, (cell_length / -2f) + 0.5f, 0);

        for (int i = 0; i < cell_width; i++)
        {
            for (int j = 0; j < cell_length; j++)
            {
                maze_grid[i, j] = Instantiate(cell, new Vector3(0, 0, 0), Quaternion.identity);

                maze_grid[i, j].transform.parent = transform;
                maze_grid[i, j].transform.localPosition = new Vector3(i, j, 0);
            }
        }

        GenerateCell(null, maze_grid[0, 0]);
    }

    private void GenerateCell(MazeCellScript prev_cell, MazeCellScript next_cell)
    {
        next_cell.Visit();
        ClrWalls(prev_cell, next_cell);

        MazeCellScript new_cell;
        
        do
        {
            new_cell = GetNextCell(next_cell);

            if (new_cell != null)
            {
                GenerateCell(next_cell, new_cell);
            }
        } while (new_cell != null);
        
    }

    private void ClrWalls(MazeCellScript prev_cell, MazeCellScript next_cell)
    {
        if (prev_cell == null)
        {
            return;
        }

        // Left to Right
        if (prev_cell.transform.position.x < next_cell.transform.position.x)
        {
            prev_cell.ClrWall(MazeCellScript.WallType.Right);
            next_cell.ClrWall(MazeCellScript.WallType.Left);
            return;
        }

        // Right to left
        if (prev_cell.transform.position.x > next_cell.transform.position.x)
        {
            prev_cell.ClrWall(MazeCellScript.WallType.Left);
            next_cell.ClrWall(MazeCellScript.WallType.Right);
            return;
        }

        // Bottom to Top
        if (prev_cell.transform.position.y < next_cell.transform.position.y)
        {
            prev_cell.ClrWall(MazeCellScript.WallType.Top);
            next_cell.ClrWall(MazeCellScript.WallType.Bottom);
            return;
        }

        // Top to Bottom
        if (prev_cell.transform.position.y > next_cell.transform.position.y)
        {
            prev_cell.ClrWall(MazeCellScript.WallType.Bottom);
            next_cell.ClrWall(MazeCellScript.WallType.Top);
            return;
        }
    }

    private MazeCellScript GetNextCell(MazeCellScript curr_cell)
    {
        var next_cells = GetUnvisitedCells(curr_cell);

        return next_cells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCellScript> GetUnvisitedCells(MazeCellScript curr_cell)
    {
        int x = (int)curr_cell.transform.localPosition.x;
        int y = (int)curr_cell.transform.localPosition.y;

        if (x + 1 < cell_width)
        {
            if (!maze_grid[x + 1, y].IsVisited) 
                yield return maze_grid[x + 1, y];
        }

        if (x - 1 >= 0)
        {
            if (!maze_grid[x - 1, y].IsVisited)
                yield return maze_grid[x - 1, y];
        }

        if (y + 1 < cell_length)
        {
            if (!maze_grid[x, y + 1].IsVisited)
                yield return maze_grid[x, y + 1];
        }

        if (y - 1 >= 0)
        {
            if (!maze_grid[x, y - 1].IsVisited)
                yield return maze_grid[x, y - 1];
        }
    }
}
