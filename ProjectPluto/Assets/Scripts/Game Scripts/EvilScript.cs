using UnityEngine;

public class EvilScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Hit Player1!");
        }
        else if (collision.collider.CompareTag("Player2"))
        {
            Debug.Log("Hit Player2!");
        }
    }
}
