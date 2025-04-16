using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;

    private float horizontal;
    private float vertical;

    [SerializeField] private float move_speed;

    GameManager gm;

    bool IsInitializing = true;

    public void Init(GameManager new_gm)
    {
        rb = GetComponent<Rigidbody2D>();

        if (new_gm != null) gm = new_gm;
    }

    public void PlayerStart()
    {
        IsInitializing = false;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (IsInitializing) return;

        if (horizontal != 0 && vertical != 0)
        {
            horizontal *= 0.7f;
            vertical *= 0.7f;
        }

        rb.linearVelocity = new Vector2(horizontal * move_speed * GameManager.gameTimeScale, vertical * move_speed * GameManager.gameTimeScale);

    }
}
