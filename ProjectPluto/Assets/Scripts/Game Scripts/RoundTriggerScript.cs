using UnityEngine;

public class RoundTriggerScript : MonoBehaviour
{
    Collider2D blocker;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (blocker) return;

        if (!other.CompareTag("Player"))
        {
            blocker = other;
        } else
        {
            //Do Something Here
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
            //Do Something Here
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other == blocker)
        {
            blocker = null;
        }
    }
}
