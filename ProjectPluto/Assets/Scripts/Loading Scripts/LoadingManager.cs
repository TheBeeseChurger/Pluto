using TMPro;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public LoadingTip current_tip;

    [SerializeField] private TextMeshProUGUI random_text;
    [SerializeField] private SpriteRenderer random_sprite;

    [ContextMenu("Update Tip")]
    public void UpdateLoadingTip()
    {
        if (current_tip == null)
            return;

        random_text.text = current_tip.text;
        random_sprite.sprite = current_tip.sprite;
    }

    public Awaitable FadeSceneOut()
    {

    }
}
