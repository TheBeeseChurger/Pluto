using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    private const int SCENE_LOADING = 1;
    private const int SCENE_CREDITS = 2;
    private const int SCENE_MENU = 3;
    private const int SCENE_GAMEIDLE = 4;
    private const int SCENE_INTERMEDIARY = 5;
    private const int SCENE_GAME = 6;

    private OpeningScript _opening_script = null;
    private MenuManagerScript _menu_manager_script = null;
    private LoadingManager _loading_manager = null;
    private IntermediaryManager _intermediary_manager = null;
    private GameManager _game_manager = null;

    [SerializeField] private DataScript d_prefab;
    [SerializeField] private GameObject a_prefab;

    static GameObject audio_head;
    static DataScript data;

    [SerializeField] private bool Skip_Credits;

    public static Action MenuToIntermediary;
    public static Action IntermediaryToGame;
    public static Action GameToGame;
    public static Action GameToMenu;

    private async void Start()
    {
        await InitStatics();
        InitActions();

        if (!Skip_Credits)
        {
            await CreditsSceneInit();
            await _opening_script.RunCredits(audio_head);

            await LoadingSceneInit();

            await CreditsSceneClose();
        }
        else
        {
            await LoadingSceneInit();
        }

        await MenuSceneInit();
        await MenuSceneStart();
    }

    private async Awaitable CreditsSceneInit()
    {
        await SceneManager.LoadSceneAsync(SCENE_CREDITS, LoadSceneMode.Additive);

        _opening_script = FindFirstObjectByType<OpeningScript>();
    }

    private async Awaitable CreditsSceneClose()
    {
        _opening_script = null;

        await SceneManager.UnloadSceneAsync(SCENE_CREDITS);
    }

    private async Awaitable MenuSceneStart()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SCENE_MENU));
        await _menu_manager_script.MenuPreStart(data, audio_head);
        await Awaitable.NextFrameAsync();
        _menu_manager_script.MenuStart();
    }

    private async Awaitable MenuSceneInit()
    {
        await SceneManager.LoadSceneAsync(SCENE_MENU, LoadSceneMode.Additive);

        _menu_manager_script = FindFirstObjectByType<MenuManagerScript>();
    }

    private async Awaitable MenuSceneClose()
    {
        _menu_manager_script = null;

        await SceneManager.UnloadSceneAsync(SCENE_MENU);
    }

    private async Awaitable IntermediarySceneInit()
    {
        await SceneManager.LoadSceneAsync(SCENE_INTERMEDIARY, LoadSceneMode.Additive);

        _intermediary_manager = FindFirstObjectByType<IntermediaryManager>();
    }

    private async Awaitable IntermediaryScenePreStart()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SCENE_INTERMEDIARY));
        await _intermediary_manager.IntermediaryPreStart(data, audio_head);
    }

    private async Awaitable IntermediarySceneStart()
    {
        await Awaitable.MainThreadAsync();
        await Awaitable.WaitForSecondsAsync(1f);
        _intermediary_manager.IntermediaryStart();
    }

    private async Awaitable IntermediarySceneClose()
    {
        _intermediary_manager = null;

        await SceneManager.UnloadSceneAsync(SCENE_INTERMEDIARY);
    }

    private async Awaitable GameSceneInit()
    {
        await SceneManager.LoadSceneAsync(SCENE_GAME, LoadSceneMode.Additive);

        _game_manager = FindFirstObjectByType<GameManager>();
    }

    private async Awaitable GameScenePreStart()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SCENE_GAME));
        await _game_manager.GamePreStart(data, audio_head);
    }

    private async Awaitable GameSceneStart()
    {
        await Awaitable.MainThreadAsync();
        await Awaitable.WaitForSecondsAsync(1f);
        _game_manager.GameStart();
    }

    private async Awaitable GameSceneClose()
    {
        _game_manager = null;

        await SceneManager.UnloadSceneAsync(SCENE_GAME);
    }

    private async Awaitable LoadingSceneInit()
    {
        await SceneManager.LoadSceneAsync(SCENE_LOADING, LoadSceneMode.Additive);

        _loading_manager = FindFirstObjectByType<LoadingManager>();
    }

    private async void LoadingSceneMenuToIntermediary()
    {
        await _loading_manager.FadeSceneIn(true);
        await MenuSceneClose();
        _loading_manager.UpdateProgress(0.5f * 100f);
        await IntermediarySceneInit();
        await Awaitable.WaitForSecondsAsync(0.1f);
        _loading_manager.UpdateProgress(0.75f * 100f);
        await IntermediaryScenePreStart();
        await Awaitable.WaitForSecondsAsync(0.5f);
        _loading_manager.UpdateProgress((1f) * 100f);
        await _loading_manager.FinishProgress(true);
        await IntermediarySceneStart();
    }

    private async void LoadingSceneIntermediaryToGame()
    {
        await _loading_manager.FadeSceneIn(true);
        await IntermediarySceneClose();
        _loading_manager.UpdateProgress(0.2f * 100f);
        await GameSceneInit();
        await Awaitable.WaitForSecondsAsync(0.1f);
        _loading_manager.UpdateProgress(0.5f * 100f);
        await GameScenePreStart();
        await Awaitable.WaitForSecondsAsync(0.5f);
        _loading_manager.UpdateProgress((1f) * 100f);
        await _loading_manager.FinishProgress(true);
        await GameSceneStart();
    }

    private async void LoadingSceneGameToGame()
    {
        await _loading_manager.FadeSceneIn(true);
        await GameSceneClose();
        _loading_manager.UpdateProgress(0.2f * 100f);
        await GameSceneInit();
        await Awaitable.WaitForSecondsAsync(0.1f);
        _loading_manager.UpdateProgress(0.5f * 100f);
        await GameScenePreStart();
        await Awaitable.WaitForSecondsAsync(0.5f);
        _loading_manager.UpdateProgress(1f * 100f);
        await _loading_manager.FinishProgress(true);
        await GameSceneStart();
    }
    private async Awaitable InitStatics()
    {
        await InstantiateAsync(d_prefab);
        await InstantiateAsync(a_prefab);

        await Awaitable.MainThreadAsync();
        data = GameObject.FindWithTag("data").GetComponent<DataScript>();
        audio_head = GameObject.FindWithTag("audio");
    }

    private async void InitActions()
    {
        await Awaitable.MainThreadAsync();

        MenuToIntermediary += LoadingSceneMenuToIntermediary;
        IntermediaryToGame += LoadingSceneIntermediaryToGame;
        GameToGame += LoadingSceneGameToGame;
    }
}
