using UnityEngine;

public class RoundTriggerScript : MonoBehaviour
{
    Collider2D blocker;
    GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (blocker) return;

        if (!other.CompareTag("Player"))
        {
            blocker = other;
        } else
        {
            gameManager.NextRound();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (blocker) return;

        if (!other.CompareTag("Player"))
        {
            blocker = other;
        } else
        {
            gameManager.NextRound();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other == blocker)
        {
            blocker = null;
        }
    }

    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
    }
}
