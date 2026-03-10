using UnityEngine;

public class RoundTriggerScript : MonoBehaviour
{
    Collider2D blocker;
    GameManager gameManager;

    bool hit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (blocker) return;

        if (!other.CompareTag("Player"))
        {
            blocker = other;
        } else
        {
            if (!hit)
            {
                hit = true;
                PlayerHit();
            }
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
            if (!hit)
            {
                hit = true;
                PlayerHit();
            }
        }
    }

    private async void PlayerHit()
    {
        await Awaitable.EndOfFrameAsync();

        if (blocker) return;

        gameManager.NextRound();
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
