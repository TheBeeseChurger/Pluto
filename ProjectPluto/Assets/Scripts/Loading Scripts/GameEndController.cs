using TMPro;
using UnityEngine;

public class GameEndController : MonoBehaviour
{
    [SerializeField] Animator _round_end;
    [SerializeField] Animator _game_over;

    [SerializeField] TextMeshProUGUI _curr_round;
    [SerializeField] TextMeshProUGUI _next_round;

    public void Init()
    {
        _curr_round.text = GameManager.round.ToString();
        _next_round.text = (GameManager.round + 1).ToString();
    }

    public void PlayRoundEnd()
    {
        Destroy(_game_over.gameObject);
        _round_end.Play("Round Change");
    }

    public void PlayGameOver()
    {
        Destroy(_round_end.gameObject);
        _game_over.Play("Game Over");
    }
}
