using UnityEngine;

public class EvilScript : MonoBehaviour
{
    [SerializeField] GameManager gm;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            gm.EndGame();
        }
        else if (collision.collider.CompareTag("Player2"))
        {
            gm.EndGame();
        }
    }
}
