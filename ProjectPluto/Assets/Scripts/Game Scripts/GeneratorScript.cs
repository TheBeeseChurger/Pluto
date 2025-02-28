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

    [SerializeField] private int alt_clrs;
    [SerializeField] private int landmark_count;

    private GameObject player1;
    private GameObject player2;

    private enum NextCellFlags
    {
        Unvisited,
        Neighboring
    }

    private void Start()
    {
        MazeGenerationStage1();
        Debug.Log("Generation Stage 1: Done!");

        MazeGenerationStage2();
        Debug.Log("Generation Stage 2: Done!");

        MazeGenerationStage3();
        Debug.Log("Generation Stage 3: Done!");
    }

    private void MazeGenerationStage3()
    {
        //Randomly place Landmarks in maze based on size of maze
        for (int i = 0; i < landmark_count; i++)
        {
            int randx = Random.Range(0, cell_width - 1);
            int randy = Random.Range(0, cell_length - 1);

            var cell1 = maze_grid[randx, randy];
            var cell2 = maze_grid[randx + 1, randy];
            var cell3 = maze_grid[randx + 1, randy + 1];
            var cell4 = maze_grid[randx, randy + 1];

            ClrWalls(cell1, cell2);
            ClrWalls(cell2, cell3);
            ClrWalls(cell3, cell4);
            ClrWalls(cell4, cell1);
        }
    }

    private void MazeGenerationStage2()
    {
        //Randomly clear new walls between cells
        for (int i = 0; i < alt_clrs; i++)
        {
            int randx = Random.Range(0, cell_width);
            int randy = Random.Range(0, cell_length);

            var cell = maze_grid[randx, randy];
            var neighbor = GetNextCell(cell, NextCellFlags.Neighboring);

            ClrWalls(cell, neighbor);
        }
    }

    private void MazeGenerationStage1()
    {
        //Initialize Maze, Create basic paths
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

    private void PaintWalls(MazeCellScript prev_cell, MazeCellScript next_cell, MazeCellScript.WallColor color)
    {
        if (prev_cell == null)
        {
            return;
        }

        // Left to Right
        if (prev_cell.transform.position.x < next_cell.transform.position.x)
        {
            prev_cell.PaintWall(MazeCellScript.WallType.Right, color);
            next_cell.PaintWall(MazeCellScript.WallType.Left, color);
            return;
        }

        // Right to left
        if (prev_cell.transform.position.x > next_cell.transform.position.x)
        {
            prev_cell.PaintWall(MazeCellScript.WallType.Left, color);
            next_cell.PaintWall(MazeCellScript.WallType.Right, color);
            return;
        }

        // Bottom to Top
        if (prev_cell.transform.position.y < next_cell.transform.position.y)
        {
            prev_cell.PaintWall(MazeCellScript.WallType.Top, color);
            next_cell.PaintWall(MazeCellScript.WallType.Bottom, color);
            return;
        }

        // Top to Bottom
        if (prev_cell.transform.position.y > next_cell.transform.position.y)
        {
            prev_cell.PaintWall(MazeCellScript.WallType.Bottom, color);
            next_cell.PaintWall(MazeCellScript.WallType.Top, color);
            return;
        }
    }

    private MazeCellScript GetNextCell(MazeCellScript curr_cell, NextCellFlags flag = NextCellFlags.Unvisited)
    {
        IEnumerable<MazeCellScript> next_cells;

        if (flag == NextCellFlags.Unvisited)
        {
            next_cells = GetUnvisitedCells(curr_cell);
        }
        else if (flag == NextCellFlags.Neighboring)
        {
            next_cells = GetNeighboringCells(curr_cell);
        }
        else
        {
            next_cells = null;
        }

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

    private IEnumerable<MazeCellScript> GetNeighboringCells(MazeCellScript curr_cell)
    {
        int x = (int)curr_cell.transform.localPosition.x;
        int y = (int)curr_cell.transform.localPosition.y;

        if (x + 1 < cell_width)
        {
            yield return maze_grid[x + 1, y];
        }

        if (x - 1 >= 0)
        {
            yield return maze_grid[x - 1, y];
        }

        if (y + 1 < cell_length)
        {
            yield return maze_grid[x, y + 1];
        }

        if (y - 1 >= 0)
        {
            yield return maze_grid[x, y - 1];
        }
    }

    private void Update()
    {
        //player1 = GameObject.FindGameObjectWithTag("Player1");
        //player2 = GameObject.FindGameObjectWithTag("Player2");


    }
}
