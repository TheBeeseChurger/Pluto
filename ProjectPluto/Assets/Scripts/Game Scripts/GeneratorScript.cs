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

    private int tile_count;

    private MazeCellScript[,] maze_grid;

    [SerializeField] private float clr_percent;
    [SerializeField] private float landmark_percent;

    private MazeCellScript player1_spawn;
    private MazeCellScript player2_spawn;
    private MazeCellScript evil_spawn;

    [SerializeField] private GameObject[] landmarks;

    private enum NextCellFlags
    {
        Unvisited,
        Neighboring
    }

    public void Generate()
    {
        MazeGenerationStage1();
        Debug.Log("Generation Stage 1: Done!");

        MazeGenerationStage2();
        Debug.Log("Generation Stage 2: Done!");

        MazeGenerationStage3();
        Debug.Log("Generation Stage 3: Done!");
    }

    public MazeCellScript GetPlayer1Spawn()
    {
        return player1_spawn;
    }

    public MazeCellScript GetPlayer2Spawn()
    {
        return player2_spawn;
    }

    public MazeCellScript GetEvilSpawn()
    {
        return evil_spawn;
    }

    public MazeCellScript GetCell(int x, int y)
    {
        return maze_grid[x, y];
    }

    public void OverrideCell(MazeCellScript cell, int x, int y)
    {
        if (x < 0 || y < 0)
            return;
        
        if (x < cell_width && y < cell_length)
        {
            Destroy(maze_grid[x, y].gameObject);
            maze_grid[x, y] = cell;
        }
    }

    private void MazeGenerationStage3()
    {
        //Randomly place Landmarks in maze based on size of maze
        int landmark_count;

        landmark_count = (int)((tile_count / 4f) * (landmark_percent / 100f));

        Debug.Log("Spawning " +  landmark_count + " landmarks in maze.");

        for (int i = 0; i < landmark_count; i++)
        {
            int randx = Random.Range(0, cell_width - 1);
            int randy = Random.Range(0, cell_length - 1);

            var cell1 = maze_grid[randx, randy];
            var cell2 = maze_grid[randx + 1, randy];
            var cell3 = maze_grid[randx + 1, randy + 1];
            var cell4 = maze_grid[randx, randy + 1];

            int attempts = 3;
            while (!EmptyLocation(cell1,cell2,cell3,cell4) && attempts > 0)
            {
                randx = Random.Range(0, cell_width - 1);
                randy = Random.Range(0, cell_length - 1);

                cell1 = maze_grid[randx, randy];
                cell2 = maze_grid[randx + 1, randy];
                cell3 = maze_grid[randx + 1, randy + 1];
                cell4 = maze_grid[randx, randy + 1];

                attempts--;
            }

            if (attempts <= 0)
            {
                Debug.LogError("Unable to find spot for new landmark. Map possibly full?");
                break;
            }

            cell1.Lock();
            cell2.Lock();
            cell3.Lock();
            cell4.Lock();

            ClrWalls(cell1, cell2);
            ClrWalls(cell2, cell3);
            ClrWalls(cell3, cell4);
            ClrWalls(cell4, cell1);

            SpawnLandmark(randx, randy);
        }
    }

    private void MazeGenerationStage2()
    {
        //Randomly clear new walls between cells and choose spawn locations
        int alt_clrs;

        alt_clrs = (int)(tile_count * (clr_percent / 100f));

        Debug.Log("Breaking up to " +  alt_clrs + " walls in maze.");

        for (int i = 0; i < alt_clrs; i++)
        {
            int randx = Random.Range(0, cell_width);
            int randy = Random.Range(0, cell_length);

            var cell = maze_grid[randx, randy];
            var neighbor = GetNextCell(cell, NextCellFlags.Neighboring);

            ClrWalls(cell, neighbor);
        }

        int plyx = Random.Range(0, cell_width);
        int plyy = Random.Range(0, cell_length);

        player1_spawn = maze_grid[plyx, plyy];

        player2_spawn = FindCellOutOfRange(plyx, plyy, 5);

        evil_spawn = FindCellOutOfRange((int)player2_spawn.transform.localPosition.x, (int)player2_spawn.transform.localPosition.y, 5);

        player1_spawn.See();
        player2_spawn.See();
        evil_spawn.See();
    }

    private void MazeGenerationStage1()
    {
        //Initialize Maze, Create basic paths
        maze_grid = new MazeCellScript[cell_width, cell_length];

        tile_count = cell_length * cell_width;

        Debug.Log("Generating " + tile_count + " cells in maze.");

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

    private void SpawnLandmark(int x, int y)
    {
        var chosen_landmark = Instantiate(landmarks[Random.Range(0, landmarks.Length)], new Vector3(0, 0, 0), Quaternion.identity);

        chosen_landmark.transform.parent = transform;
        chosen_landmark.transform.localPosition = new Vector3(x, y, 0);

        chosen_landmark.GetComponent<LandmarkCellScript>().Initialize();
    }

    private bool EmptyLocation(MazeCellScript bl_cell, MazeCellScript br_cell, MazeCellScript tl_cell, MazeCellScript tr_cell)
    {
        return !(bl_cell.IsLocked || br_cell.IsLocked || tl_cell.IsLocked || tr_cell.IsLocked);
    }

    private MazeCellScript FindCellOutOfRange(int x, int y, int range)
    {
        MazeCellScript found_cell;

        int randx = Random.Range(0, cell_width);
        int randy = Random.Range(0, cell_length);

        int attempts = 3;
        while ((randx > x - range && randx < x + range) && (randy > y - range && randy < y + range) && attempts > 0)
        {
            randx = Random.Range(0, cell_width);
            randy = Random.Range(0, cell_length);

            attempts--;
        }

        if (attempts <= 0)
        {
            Debug.LogError("Unable to find cell outside of range. Map possibly too small?");
        }

        found_cell = maze_grid[randx, randy];

        return found_cell;
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

    public IEnumerable<MazeCellScript> GetConnectedCells(MazeCellScript curr_cell)
    {
        int x;
        int y;

        if (!curr_cell.IsLandmarkCell)
        {
            x = (int)curr_cell.transform.localPosition.x;
            y = (int)curr_cell.transform.localPosition.y;
        }
        else
        {
            x = (int)(curr_cell.transform.parent.localPosition.x + curr_cell.transform.localPosition.x);
            y = (int)(curr_cell.transform.parent.localPosition.y + curr_cell.transform.localPosition.y);
        }

        if (curr_cell.IsWallClr(MazeCellScript.WallType.Right) && x + 1 < cell_width)
        {
            yield return maze_grid[x + 1, y];
        }

        if (curr_cell.IsWallClr(MazeCellScript.WallType.Left) && x - 1 >= 0)
        {
            yield return maze_grid[x - 1, y];
        }

        if (curr_cell.IsWallClr(MazeCellScript.WallType.Top) && y + 1 < cell_length)
        {
            yield return maze_grid[x, y + 1];
        }

        if (curr_cell.IsWallClr(MazeCellScript.WallType.Bottom) && y - 1 >= 0)
        {
            yield return maze_grid[x, y - 1];
        }
    }
}
