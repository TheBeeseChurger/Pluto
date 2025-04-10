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

    [SerializeField] private DataScript d_prefab;
    [SerializeField] private GameObject a_prefab;

    static GameObject audio_head;
    static DataScript data;

    [SerializeField] private bool Skip_Credits;

    private async void Start()
    {
        await InitStatics();

        if (!Skip_Credits)
        {
            await SceneManager.LoadSceneAsync(SCENE_CREDITS, LoadSceneMode.Additive);
            CreditsSceneInit();
            await _opening_script.RunCredits(audio_head);

            await SceneManager.LoadSceneAsync(SCENE_LOADING, LoadSceneMode.Additive);
            await LoadingSceneInit();

            await CreditsSceneClose();
        }
        else
        {
            await SceneManager.LoadSceneAsync(SCENE_LOADING, LoadSceneMode.Additive);
            await LoadingSceneInit();
        }


        await LoadingSceneMenuOpen();

    }

    private void CreditsSceneInit()
    {
        _opening_script = FindFirstObjectByType<OpeningScript>();
    }

    private async Awaitable CreditsSceneClose()
    {
        _opening_script = null;

        await SceneManager.UnloadSceneAsync(SCENE_CREDITS);
    }

    public async Awaitable MenuSceneStart()
    {
        _menu_manager_script = FindFirstObjectByType<MenuManagerScript>();

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SCENE_MENU));
        await _menu_manager_script.MenuStart(data, audio_head);
    }

    private async Awaitable MenuSceneInit()
    {
        await SceneManager.LoadSceneAsync(SCENE_MENU, LoadSceneMode.Additive);
    }

    private async Awaitable LoadingSceneInit()
    {
        _loading_manager = FindFirstObjectByType<LoadingManager>();

        await _loading_manager.FadeSceneIn(true);
    }

    private async Awaitable LoadingSceneMenuOpen()
    {
        await MenuSceneInit();
        _loading_manager.UpdateProgress((1f / 3f) * 100f);
        await MenuSceneStart();
        await Awaitable.WaitForSecondsAsync(0.1f);
        _loading_manager.UpdateProgress((2f / 3f) * 100f);
        await Awaitable.WaitForSecondsAsync(0.5f);
        _loading_manager.UpdateProgress((1f) * 100f);
        await _loading_manager.FinishProgress(true);
    }

    private async Awaitable InitStatics()
    {
        await InstantiateAsync(d_prefab);
        await InstantiateAsync(a_prefab);

        await Awaitable.MainThreadAsync();
        data = GameObject.FindWithTag("data").GetComponent<DataScript>();
        audio_head = GameObject.FindWithTag("audio");
    }
}
