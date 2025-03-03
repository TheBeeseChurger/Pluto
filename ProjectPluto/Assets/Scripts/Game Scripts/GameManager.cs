using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GeneratorScript maze_gen;
    [SerializeField] CameraTraceScript tracer;

    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    void Start()
    {
        maze_gen.Generate();

        var p1_cell = maze_gen.GetPlayer1Spawn();
        var p2_cell = maze_gen.GetPlayer2Spawn();

        player1.transform.position = new Vector3(p1_cell.transform.position.x,p1_cell.transform.position.y, player1.transform.position.z);
        player2.transform.position = new Vector3(p2_cell.transform.position.x, p2_cell.transform.position.y, player2.transform.position.z);

        tracer.CamReset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
