using TMPro;
using Unity.VisualScripting;
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

    [SerializeField] private CameraFollowScript cam_follow;

    [Header("Initialization References")]
    [SerializeField] GameObject _game_animators;
    
    GameEndController _end_controller;

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

    public async Awaitable ControllerInit()
    {
        await InstantiateAsync(_game_animators);

        _end_controller = FindFirstObjectByType<GameEndController>();
        _end_controller.GetComponent<CameraFollowScript>().Init(true);
        _end_controller.Init();
    }

    public async Awaitable PlayAnimation(bool game_over)
    {
        if (_end_controller == null) return;

        if (game_over) _end_controller.PlayGameOver();
        else _end_controller.PlayRoundEnd();

        await Awaitable.WaitForSecondsAsync(12f);
    }

    public async Awaitable FadeBlankSceneOut(bool audio_change = false)
    {
        float curr_time = 0f;
        while (scene_background.color.a > 0f)
        {
            var alpha = Mathf.Lerp(1f, 0f, curr_time / 1f);

            scene_background.color = new Color(scene_background.color.r, scene_background.color.g, scene_background.color.b, alpha);
            if (audio_change) audio_volume.SetFloat("Volume", ((1f - alpha) * 80f) - 80f);

            await Awaitable.NextFrameAsync();
            curr_time += Time.deltaTime;
        }

        cam_follow.Init(false);

        if(_end_controller != null) Destroy(_end_controller.gameObject);
    }

    public async Awaitable FadeBlankSceneIn(bool audio_change = false)
    {
        cam_follow.Init(true);

        float curr_time = 0f;
        while (scene_background.color.a < 1f)
        {
            var alpha = Mathf.Lerp(0f, 1f, curr_time / 1f);

            scene_background.color = new Color(scene_background.color.r, scene_background.color.g, scene_background.color.b, alpha);
            if (audio_change) audio_volume.SetFloat("Volume", ((1f - alpha) * 80f) - 80f);

            await Awaitable.NextFrameAsync();
            curr_time += Time.deltaTime;
        }
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

        cam_follow.Init(false);
    }

    public async Awaitable FadeSceneIn(bool audio_change = false)
    {
        SceneReset();
        PickNewLoadingTip();
        UpdateLoadingTip();
        cam_follow.Init(true);

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

    private void SceneReset()
    {
        scene_alpha.alpha = 0f;
        scene_background.color = new Color(scene_background.color.r, scene_background.color.g, scene_background.color.b, 0f);
        scene_icon.color = new Color(scene_icon.color.r, scene_icon.color.g, scene_icon.color.b, 0f);
        progress = 0f;
        bar.localScale = new Vector3(0f, bar.localScale.y, bar.localScale.z);

        CTC_alpha.alpha = 0f;
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
