using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class LoadingManager : MonoBehaviour
{
    public LoadingTip current_tip;

    [SerializeField] private TextMeshProUGUI random_text;
    [SerializeField] private SpriteRenderer random_sprite;

    [SerializeField] private CanvasGroup scene_alpha;
    [SerializeField] private SpriteRenderer scene_background;
    [SerializeField] private SpriteRenderer scene_icon;

    [SerializeField] private AudioMixer audio_volume;

    [ContextMenu("Update Tip")]
    public void UpdateLoadingTip()
    {
        if (current_tip == null)
            return;

        random_text.text = current_tip.text;
        random_sprite.sprite = current_tip.sprite;
    }

    public async Awaitable FadeSceneOut()
    {
        float curr_time = 0f;
        while (scene_alpha.alpha > 0f)
        {
            var alpha = Mathf.Lerp(1f, 0f, curr_time / 1f);

            scene_alpha.alpha = alpha;
            scene_background.color = new Color(scene_background.color.r, scene_background.color.g, scene_background.color.b, alpha);
            scene_icon.color = new Color(scene_icon.color.r, scene_icon.color.g, scene_icon.color.b, alpha);
            //audio_volume.SetFloat("Volume",0);

            await Awaitable.NextFrameAsync();
            curr_time += Time.deltaTime;
        }
    }

    public async Awaitable FadeSceneIn()
    {
        float curr_time = 0f;
        while (scene_alpha.alpha < 1f)
        {
            var alpha = Mathf.Lerp(0f, 1f, curr_time / 1f);

            scene_alpha.alpha = alpha;
            scene_background.color = new Color(scene_background.color.r, scene_background.color.g, scene_background.color.b, alpha);
            scene_icon.color = new Color(scene_icon.color.r, scene_icon.color.g, scene_icon.color.b, alpha);

            await Awaitable.NextFrameAsync();
            curr_time += Time.deltaTime;
        }
    }
}
