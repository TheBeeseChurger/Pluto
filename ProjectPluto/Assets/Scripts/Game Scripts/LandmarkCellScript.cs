using UnityEngine;

public class LandmarkCellScript : MonoBehaviour
{
    [SerializeField] MazeCellScript cell_bl;
    [SerializeField] MazeCellScript cell_br;
    [SerializeField] MazeCellScript cell_tl;
    [SerializeField] MazeCellScript cell_tr;

    private GeneratorScript maze;

    private int maze_x;
    private int maze_y;

    public void Initialize()
    {
        maze = GameObject.Find("Maze").GetComponent<GeneratorScript>();

        maze_x = (int)transform.localPosition.x;
        maze_y = (int)transform.localPosition.y;

        
    }

    public void CopyCells()
    {

    }
}
