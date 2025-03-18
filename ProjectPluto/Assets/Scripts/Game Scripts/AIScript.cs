using UnityEngine;

public class AIScript : MonoBehaviour
{
    private float horizontal;
    private float vertical;

    [SerializeField] private float move_speed;

    [SerializeField] GameManager gm;

    [SerializeField] private AIType type;

    public enum AIType
    {
        p1,
        p2,
        evil
    }

    void Update()
    {
        //TODO: Make Ai Input
        horizontal = 1f;
        vertical = 0f;
    }

    private void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0)
        {
            horizontal *= 0.7f;
            vertical *= 0.7f;
        }

        transform.position += new Vector3(horizontal * move_speed * 0.01f, vertical * move_speed * 0.01f, 0f);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(type)
        {
            case AIType.p1:
                if (collision.collider.CompareTag("Player2"))
                {
                    gm.NextRound();
                }

                if (collision.collider.CompareTag("Evil"))
                {
                    gm.EndGame();
                }

                break;
            case AIType.p2:
                if (collision.collider.CompareTag("Player"))
                {
                    gm.NextRound();
                }

                if (collision.collider.CompareTag("Evil"))
                {
                    gm.EndGame();
                }

                break;
            case AIType.evil:
                if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Player2"))
                {
                    gm.EndGame();
                }

                break;
        }
    }
}
