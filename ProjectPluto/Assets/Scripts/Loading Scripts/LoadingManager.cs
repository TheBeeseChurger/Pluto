using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class LoadingManager : MonoBehaviour
{
    public LoadingTip[] loading_tips;
    private LoadingTip current_tip;

    [SerializeField] private TextMeshProUGUI random_text;
    [SerializeField] private SpriteRenderer random_sprite;

    [SerializeField] private CanvasGroup scene_alpha;
    [SerializeField] private SpriteRenderer scene_background;
    [SerializeField] private SpriteRenderer scene_icon;
    [SerializeField] private CanvasGroup CTC_alpha;

    [SerializeField] private AudioMixer audio_volume;

    [Range(0f, 1f)]
    private float progress = 0f;
    [SerializeField] private RectTransform bar;

    private bool CTC;

    InputSystem_Actions _input_system = null;

    [ContextMenu("Update Tip")]
    public void UpdateLoadingTip()
    {
        if (current_tip == null)
            return;

        random_text.text = current_tip.text;
        random_sprite.sprite = current_tip.sprite;
    }

    public async Awaitable FadeSceneOut(bool audio_change = false)
    {
        float curr_time = 0f;
        while (scene_alpha.alpha > 0f)
        {
            var alpha = Mathf.Lerp(1f, 0f, curr_time / 1f);

            scene_alpha.alpha = alpha;
            scene_background.color = new Color(scene_background.color.r, scene_background.color.g, scene_background.color.b, alpha);
            scene_icon.color = new Color(scene_icon.color.r, scene_icon.color.g, scene_icon.color.b, alpha);
            if (audio_change) audio_volume.SetFloat("Volume", ((1f - alpha) * 80f) - 80f);

            await Awaitable.NextFrameAsync();
            curr_time += Time.deltaTime;
        }
    }

    public async Awaitable FadeSceneIn(bool audio_change = false)
    {
        PickNewLoadingTip();
        UpdateLoadingTip();

        float curr_time = 0f;
        while (scene_alpha.alpha < 1f)
        {
            var alpha = Mathf.Lerp(0f, 1f, curr_time / 1f);

            scene_alpha.alpha = alpha;
            scene_background.color = new Color(scene_background.color.r, scene_background.color.g, scene_background.color.b, alpha);
            scene_icon.color = new Color(scene_icon.color.r, scene_icon.color.g, scene_icon.color.b, alpha);
            if (audio_change) audio_volume.SetFloat("Volume", ((1f - alpha) * 80f) - 80f);

            await Awaitable.NextFrameAsync();
            curr_time += Time.deltaTime;
        }
    }

    public async void UpdateProgress(float new_progress)
    {
        progress = new_progress;

        var time = 0f;
        while (Mathf.Abs(bar.localScale.x - progress) > 0.1f)
        {
            bar.localScale = new Vector3(Mathf.Lerp(bar.localScale.x, progress, 10.0f * Time.deltaTime), bar.localScale.y, bar.localScale.z);


            await Awaitable.NextFrameAsync();
            time += Time.deltaTime;
        }

        await Awaitable.NextFrameAsync();
    }

    public async Awaitable FinishProgress(bool audio_change = false)
    {
        while (bar.localScale.x < 99.9f)
        {
            await Awaitable.NextFrameAsync();
        }

        CTC = false;

        //Magic Happens
        _input_system ??= new();

        _input_system.UI.Cancel.performed += OnMyClick;
        _input_system.UI.Submit.performed += OnMyClick;
        _input_system.UI.Click.performed += OnMyClick;

        _input_system.UI.Cancel.Enable();
        _input_system.UI.Submit.Enable();
        _input_system.UI.Click.Enable();

        while (!CTC)
        {
            if (CTC_alpha.alpha < 1f)
            {
                CTC_alpha.alpha += 0.4f * Time.deltaTime;
            }

            await Awaitable.NextFrameAsync();
        }

        _input_system.UI.Cancel.Disable();
        _input_system.UI.Submit.Disable();
        _input_system.UI.Click.Disable();

        await FadeSceneOut(audio_change);
    }

    private void OnMyClick(InputAction.CallbackContext _context)
    {
        CTC = true;
    }

    private void PickNewLoadingTip()
    {
        current_tip = loading_tips[Random.Range(0, loading_tips.Length)];
    }
}
